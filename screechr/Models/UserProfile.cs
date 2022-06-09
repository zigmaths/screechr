using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace screechr.Models
{
    /// <summary>
    /// Defines the details of a user.
    /// </summary>
    public class UserProfile
    {
        private readonly IConfiguration _configuration;
        private const string CONST_SECRET_FOR_KEY = "Authentication:SecretForKey";
        private const string CONST_ISSUER = "Authentication:Issuer";
        private const string CONST_AUDIENCE = "Authentication:Audience";
        private string _token = null;

        /// <summary>
        /// Constructor method.
        /// </summary>
        /// <param name="configuration">Configuration settings for the application.</param>
        public UserProfile(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Generates a new token for this user profile.
        /// </summary>
        /// <returns>A new token for this user profile.</returns>
        public void GenerateNewToken()
        {
            // create a new token for user
            var securityKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(_configuration[CONST_SECRET_FOR_KEY]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", Id.ToString()));
            claimsForToken.Add(new Claim("given_name", FirstName));
            claimsForToken.Add(new Claim("family_name", LastName));
            claimsForToken.Add(new Claim("datecreated", DateCreated));

            var jwtSecurityToken = new JwtSecurityToken(
                _configuration[CONST_ISSUER],
                _configuration[CONST_AUDIENCE],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),  // expiration time
                signingCredentials);

            _token = new JwtSecurityTokenHandler()
               .WriteToken(jwtSecurityToken);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets the public name for the user (80 characters max).
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's surname.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's profile image.
        /// </summary>
        public Uri? ProfileImage { get; set; }

        /// <summary>
        /// Gets or sets the Date and time that the user was added into the system.
        /// </summary>
        //public DateTime DateCreated { get; set; }
        public string DateCreated { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the most recent date and time the user profile was updated.
        /// </summary>
        //public DateTime DateModified { get; set; }
        public string DateModified { get; set; } = string.Empty;

        /// <summary>
        /// Gets the current token used to validate requests from this user.
        /// </summary>
        public string SecretToken
        {
            get
            {
                if (_token == null)
                    GenerateNewToken();

                return _token;
            }
        }

        #endregion Properties
    }
}
