using System.ComponentModel.DataAnnotations;

namespace screechr.Models
{
    /// <summary>
    /// Defines the details of a user.
    /// </summary>
    /// <remarks
    /// Used when updating a user profile using the PATCH HTTP method.
    /// </remarks>
    public class UserProfileForUpdate
    {
        /// <summary>
        /// Gets or sets the public name for the user (80 characters max).
        /// </summary>
        [Key]
        [Required]
        [MaxLength(80)]
        //[RegularExpression()] // TODO: regex to prevent all whitespace
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for the user.
        /// </summary>
        [Required]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's first name (100 characters max).
        /// </summary>
        [Required]
        [MaxLength(100)]
        //[RegularExpression()] // TODO: regex to prevent all whitespace
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's surname (100 characters max).
        /// </summary>
        [Required]
        [MaxLength(100)]
        //[RegularExpression()] // TODO: regex to prevent all whitespace
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's profile image.
        /// </summary>
        public Uri? ProfileImage { get; set; }
    }
}
