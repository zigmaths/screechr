using System.ComponentModel.DataAnnotations;

namespace screechr.Models
{
    /// <summary>
    /// Defines the details of an update to a screech made by a user.
    /// </summary>
    /// <remarks
    /// Used when updating a screech using the PATCH HTTP method.
    /// </remarks>
    public class ScreechForUpdate
    {
        /// <summary>
        /// Gets or sets the content for the screech (1024 characters max).
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string Content { get; set; } = string.Empty;
    }
}
