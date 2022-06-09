namespace screechr.Models
{
    public class UserProfileWithoutToken
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public ulong Id { get; set; }

        /// <summary>
        /// Gets or sets the public name for the user (80 characters max).
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        // Hide the password
        ///// <summary>
        ///// Gets or sets the password for the user.
        ///// </summary>
        //public string Password { get; set; } = string.Empty;

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
    }

    #endregion
}
