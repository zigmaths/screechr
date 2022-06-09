using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using screechr.Models;
using screechr.Services;
using System.Security.Claims;

namespace screechr.Controllers
{
    //[Route("api/userprofiles/{profileid}/screeches")]
    [Route("api/screeches")]
    [ApiController]
    public class ScreechController : ControllerBase
    {
        private readonly ILogger<ScreechController> _logger;
        private readonly IUserDataRepository _userDataRepository;

        /// <summary>
        /// Constructor method.
        /// </summary>
        /// <param name="userDataRepository">The repository where user data is stored.</param>
        /// <param name="logger">Logger for diagnostics</param>
        /// <exception cref="ArgumentNullException">UserProfileDataStore or Logger is null.</exception>
        public ScreechController(IUserDataRepository userDataRepository,
            ILogger<ScreechController> logger)
        {
            _userDataRepository = userDataRepository ?? 
                throw new ArgumentNullException(nameof(userDataRepository));

            _logger = logger ?? 
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves all screeches.
        /// </summary>
        /// <returns>All screeches in the system.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Screech>>> GetAllScreeches(ulong userId)
        {
            // unauthenticated users may view all screeches
            var screeches = await _userDataRepository.GetAllScreechesAsync();
            return Ok(screeches);
        }

        /// <summary>
        /// Retrieves a screech by Id.
        /// </summary>
        /// <param name="screechId">Identifier of the screech to retrieve.</param>
        /// <returns>The requested screech, if it exists. Otherwise returns the NotFound status code.</returns>
        [HttpGet("{screechId}", Name = "GetScreech")]
        [Authorize]
        public async Task<ActionResult<Screech>> GetScreech(ulong screechId)
        {
            var screech = await _userDataRepository.GetScreechByIdAsync(screechId);
            if (screech == null)
            {
                // all authenticated users may view any other screech by Id
                _logger.LogInformation($"Screech with Id '{screechId}' not found.");
                return NotFound();
            }

            return Ok(screech);
        }

        /// <summary>
        /// Creates and stores a screech.
        /// </summary>
        /// <param name="creatorId">The creator Id for the screech.</param>
        /// <param name="screech">The screech details deserialized from the request body.</param>
        /// <returns>The <c>Screech</c> that was created.</returns>
        [Authorize]
        [HttpPost("{creatorId}")]
        public async Task<ActionResult<Screech>> CreateScreech(ulong creatorId,
            ScreechForCreation screech)
        {
            // check if profile exists for the supplied creator(user) Id
            if (!await _userDataRepository.UserProfileExistsByIdAsync(creatorId))
            {
                _logger.LogError($"Cannot create screech - creator Id '{creatorId}' not found.");
                return NotFound();
            }

            var idString = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            if (idString != null && UInt64.TryParse(idString, out ulong userIdFromClaims))
            {
                // user may create screeches only using their own profile
                if (userIdFromClaims != creatorId)
                    return Forbid();
            }
            else
            {
                _logger.LogError($"Cannot create screech - claim type '{ClaimTypes.NameIdentifier}' not found or invalid.");
                return BadRequest();
            }

            var finalScreech = await _userDataRepository.AddScreechAsync(creatorId, screech);
            //if (finalScreech == null)
            //{
            //    // should not happen
            //    _logger.LogCritical($"Cannot create screech - duplicate screech Id.");
            //    return StatusCode(500); // internal server error - duplicate key
            //}

            _logger.LogTrace($"Screech for user Id '{creatorId}' has been added.");

            return CreatedAtRoute("GetScreech",
                new
                {
                    screechId = finalScreech.Id
                },
                finalScreech);
        }

        /// <summary>
        /// Performs a full update of a screech.
        /// </summary>
        /// <param name="creatorId">The creator Id for the screech.</param>
        /// <param name="screechId">The Id of the screech to update.</param>
        /// <param name="screechForUpdate">The screech update details deserialized from the request body.</param>
        [HttpPut("{creatorId}/{screechId}")]
        [Authorize]
        public async Task<ActionResult> UpdateScreech(ulong creatorId, ulong screechId,
            ScreechForUpdate screechForUpdate)
        {
            // check if profile exists for the supplied creator(user) Id
            if (!await _userDataRepository.UserProfileExistsByIdAsync(creatorId))
            {
                _logger.LogError($"Cannot update screech - creator Id '{creatorId}' not found.");
                return NotFound();
            }

            var idString = User.Claims.FirstOrDefault(s => s.Type == ClaimTypes.NameIdentifier)?.Value;
            if (idString != null && UInt64.TryParse(idString, out ulong userIdFromClaims))
            {
                // user may update screeches only using their own profile
                if (userIdFromClaims != creatorId)
                    return Forbid();
            }
            else
            {
                _logger.LogError($"Cannot perform update on screech Id '{screechId}' - claim type '{ClaimTypes.NameIdentifier}' not found or invalid.");
                return BadRequest();
            }

            // get the screech from the store
            var screechFromStore = await _userDataRepository.GetScreechByUserAsync(creatorId, screechId);
            if (screechFromStore == null)
            {
                _logger.LogError($"Cannot perform full update - screech with Id '{screechId}' by user '{creatorId}' not found.");
                return NotFound();
            }

            // for demo purposes - update screech in data store
            screechFromStore.Content = screechForUpdate.Content;
            // update the modified date
            screechFromStore.DateModified = DateTime.UtcNow.ToString("o");

            _logger.LogTrace($"Screech with Id '{screechId}' by user '{creatorId}' fully updated.");

            return NoContent();
        }
    }
}
