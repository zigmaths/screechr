namespace screechr.Models
{
    /// <summary>
    /// Defines the details of a screech.
    /// </summary>
    public class Screech
    {
        /// <summary>
        /// Gets or sets the unique identifier for the screech.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets the content for the screech (1024 characters max).
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Id of the user that created the screech.
        /// </summary>
        public ulong CreatorId { get; set; }

        /// <summary>
        /// Gets or sets the date and time the screech was added into the system.
        /// </summary>
        //public DateTime DateCreated { get; set; }
        public string DateCreated { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the most recent date and time the screech was updated.
        /// </summary>
        //public DateTime DateModified { get; set; }
        public string DateModified { get; set; } = string.Empty;
    }
}
