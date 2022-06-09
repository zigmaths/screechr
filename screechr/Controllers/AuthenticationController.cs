using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using screechr.Models;
using screechr.Services;

namespace screechr.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserDataRepository _userDataRepository;

        /// <summary>
        /// Constructor method.
        /// </summary>
        /// <param name="configuration">Configuration settings for the application.</param>
        /// <param name="userDataRepository">The repository where user data is stored.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <c>configuration</c> is null.</exception>
        public AuthenticationController(IConfiguration configuration, IUserDataRepository userDataRepository)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
            _userDataRepository = userDataRepository ?? 
                throw new ArgumentNullException(nameof(userDataRepository));
        }

        /// <summary>
        /// Verifies the identity of the client and returns a token accordingly.
        /// </summary>
        /// <param name="authenticationRequest">The request containing the client credentials.</param>
        /// <returns>Token that the client can use in making calls to the API.</returns>
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> Authenticate(AuthenticationRequestBody authenticationRequestBody)
        {
            // validate the username/password
            var user = await ValidateUserCredentials(
                authenticationRequestBody.UserName,
                authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            string tokenToReturn = user.SecretToken;
            return Ok(tokenToReturn);
        }

        /// <summary>
        /// Validates the credentials provided by the client.
        /// </summary>
        /// <param name="userName">User name in the request body.</param>
        /// <param name="password">Password in the request body.</param>
        /// <returns>The user profile, if the credentials are valid.</returns>
        private async Task<UserProfile?> ValidateUserCredentials(string userName, string password)
        {
            // for demo purposes only
            UserProfile? userProfile = await _userDataRepository.GetUserProfileByNameAsync(userName);
            if (userProfile != null && string.Compare(userProfile.Password, password) == 0)
                return userProfile;

            return null;
        }

        /// <summary>
        /// Captures the details of an authentication request from the client.
        /// </summary>
        public class AuthenticationRequestBody
        {
            /// <summary>
            /// The user name.
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// The password associated with the user.
            /// </summary>
            public string Password { get; set; }
        }
    }
}
