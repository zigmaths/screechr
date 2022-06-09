using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using screechr.Models;
using screechr.Services;
using System.Security.Claims;

namespace screechr.Controllers
{
    [ApiController]
    [Route("api/userprofiles")]
    public class UserProfileController : ControllerBase
    {
        private readonly ILogger<UserProfileController> _logger;
        private readonly IUserDataRepository _userDataRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor method.
        /// </summary>
        /// <param name="userDataRepository">The repository where user data is stored.</param>
        /// <param name="logger">Logger for diagnostics</param>
        /// <param name="mapper">Mapper for data returned by this controller.</param>
        /// <exception cref="ArgumentNullException">UserProfileDataStore or Logger is null.</exception>
        public UserProfileController(IUserDataRepository userDataRepository,
            ILogger<UserProfileController> logger, IMapper mapper)
        {
            _userDataRepository = userDataRepository ?? throw new ArgumentNullException(nameof(userDataRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Creates and stores a user profile in the data repo.
        /// </summary>
        /// <param name="userProfile">The user profile details.</param>
        /// <returns>The user profile that was created.</returns>
        [HttpPost]
        public async Task<ActionResult<UserProfileWithoutToken>> CreateUserProfile(UserProfileForCreation userProfile)
        {
            // check if profile exists for the supplied user name - user name must be unique
            if (await _userDataRepository.UserProfileExistsByNameAsync(userProfile.UserName))
            {
                _logger.LogError($"Cannot create user profile - username '{userProfile.UserName}' is already taken.");
                return Conflict();//BadRequest(); //TODO: check which status code is more appropriate
            }

            var finalUserProfile = await _userDataRepository.AddUserProfileAsync(userProfile);
            //if (finalUserProfile == null)
            //{
            //    // should not happen
            //    _logger.LogCritical($"Cannot create user profile - duplicate profile Id.");
            //    return StatusCode(500); // internal server error - duplicate key
            //}

            _logger.LogInformation($"User profile for UserName '{userProfile.UserName}' has been added.");

            // do not return token and password information
            var createdUserProfileToReturn = _mapper.Map<UserProfileWithoutToken>(finalUserProfile);

            return CreatedAtRoute("GetUserProfile",
                new
                {
                    userId = createdUserProfileToReturn.Id
                },
                createdUserProfileToReturn);
        }

        /// <summary>
        /// Retrieves all user profiles.
        /// </summary>
        /// <returns>The profile details of all users.</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfileWithoutToken>>> GetAllUserProfiles()
        {
            // all authenticated users may view all profiles
            var userProfiles = await _userDataRepository.GetAllUserProfilesAsync();
            // do not return token and password
            return Ok(_mapper.Map<IEnumerable<UserProfileWithoutToken>>(userProfiles));
        }

        /// <summary>
        /// Retrieves a user profile using the supplied id.
        /// </summary>
        /// <param name="userId">Identifier of the user profile to retrieve.</param>
        /// <returns>The requested user profile, if it exists. Otherwise returns the NotFound status code.</returns>
        [Authorize]
        [HttpGet("{userId}", Name = "GetUserProfile")]
        public async Task<ActionResult<UserProfileWithoutToken>> GetUserProfile(ulong userId)
        {
            var userProfile = await _userDataRepository.GetUserProfileByIdAsync(userId);
            if (userProfile == null)
            {
                // all authenticated users may view any other profile by Id
                _logger.LogInformation($"User profile with Id '{userId}' not found.");
                return NotFound();
            }
            // do not return token and password
            return Ok(_mapper.Map<UserProfileWithoutToken>(userProfile));
        }

        /// <summary>
        /// Perform a partial update of a user profile.
        /// </summary>
        /// <param name="userId">The identifier of the user to partially update.</param>
        [Authorize]
        [HttpPatch("{userId}")]
        public async Task<ActionResult> PartiallyUpdateUserProfile(ulong userId,
            JsonPatchDocument<UserProfileForUpdate> patchDocument)
        {
            var idString = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            if (idString != null && UInt64.TryParse(idString, out ulong userIdFromClaims))
            {
                // user may modify only their own profile
                if (userIdFromClaims != userId)
                    return Forbid();
            }
            else
            {
                _logger.LogError($"Cannot perform partial update on profile Id '{userId} - claim type '{ClaimTypes.NameIdentifier}' not found or invalid.");
                return BadRequest();
            }

            var userProfileFromStore = await _userDataRepository.GetUserProfileByIdAsync(userId);
            if (userProfileFromStore == null)
            {
                _logger.LogError($"Cannot perform partial update - user profile with Id '{userId}' not found.");
                return NotFound();
            }

            // create patch object upon which the patch doc will be applied
            var userProfileToPatch = new UserProfileForUpdate()
            {
                UserName = userProfileFromStore.UserName,
                Password = userProfileFromStore.Password,
                FirstName = userProfileFromStore.FirstName,
                LastName = userProfileFromStore.LastName,
                ProfileImage = userProfileFromStore.ProfileImage
                //DateCreated = userProfileFromStore.DateCreated // do not update DateCreated
            };

            patchDocument.ApplyTo(userProfileToPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // validate data annotations
            if (!TryValidateModel(userProfileToPatch))
                return BadRequest(ModelState);

            // for demo purposes - update user profile in data store
            userProfileFromStore.UserName = userProfileToPatch.UserName;
            userProfileFromStore.Password = userProfileToPatch.Password;
            userProfileFromStore.FirstName = userProfileToPatch.FirstName;
            userProfileFromStore.LastName = userProfileToPatch.LastName;
            userProfileFromStore.ProfileImage = userProfileToPatch.ProfileImage;
            // update the modified date
            userProfileFromStore.DateModified = DateTime.UtcNow.ToString("o");

            _logger.LogInformation($"User profile for Id '{userId}' partially updated.");

            return NoContent();
        }

        /// <summary>
        /// Perform a full update of a user profile.
        /// </summary>
        /// <param name="userId">The identifier of the user to update.</param>
        /// <param name="userProfileForUpdate">Update details deserialized from the request.</param>
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<ActionResult> UpdateUserProfile(ulong userId,
            UserProfileForUpdate userProfileForUpdate)
        {
            var idString = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            if (idString != null && UInt64.TryParse(idString, out ulong userIdFromClaims))
            {
                // user may update only their own profile
                if (userIdFromClaims != userId)
                    return Forbid();
            }
            else
            {
                _logger.LogError($"Cannot perform full update on profile Id '{userId} - claim type '{ClaimTypes.NameIdentifier}' not found or invalid.");
                return BadRequest();
            }

            // get the user profile from the store
            var userProfileFromStore = await _userDataRepository.GetUserProfileByIdAsync(userId);
            if (userProfileFromStore == null)
            {
                _logger.LogError($"Cannot perform full update - user profile with Id '{userId}' not found.");
                return NotFound();
            }

            // for demo purposes - update user profile in data store
            userProfileFromStore.UserName = userProfileForUpdate.UserName;
            userProfileFromStore.Password = userProfileForUpdate.Password;
            userProfileFromStore.FirstName = userProfileForUpdate.FirstName;
            userProfileFromStore.LastName = userProfileForUpdate.LastName;
            userProfileFromStore.ProfileImage= userProfileForUpdate.ProfileImage;
            // update the modified date
            userProfileFromStore.DateModified= DateTime.UtcNow.ToString("o");

            _logger.LogInformation($"User profile for Id '{userId}' fully updated.");

            return NoContent();
        }
    }
}
