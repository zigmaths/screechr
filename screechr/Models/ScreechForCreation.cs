using System.ComponentModel.DataAnnotations;

namespace screechr.Models
{
    /// <summary>
    /// Defines the details of a screech created by a user.
    /// </summary>
    /// <remarks
    /// Used when creating a screech using the POST HTTP method.
    /// </remarks>
    public class ScreechForCreation
    {
        /// <summary>
        /// Gets or sets the content for the screech (1024 characters max).
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string Content { get; set; } = string.Empty;
    }
}
