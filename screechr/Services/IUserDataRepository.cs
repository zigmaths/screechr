using screechr.Models;

namespace screechr.Services
{
    /// <summary>
    /// Defines the members that a user data repository must implement.
    /// </summary>
    public interface IUserDataRepository
    {
        /// <summary>
        /// Checks whether a profile exists for the supplied user name.
        /// </summary>
        /// <param name="userName">The user name to check the profile for.</param>
        /// <returns><c>true</c> if a profile for the user name exists, false otherwise.</returns>
        Task<bool> UserProfileExistsByNameAsync(string userName);

        /// <summary>
        /// Checks whether a profile exists for the supplied Id.
        /// </summary>
        /// <param name="userId">The Id to check the profile for.</param>
        /// <returns><c>true</c> if a profile for the Id exists, false otherwise.</returns>
        Task<bool> UserProfileExistsByIdAsync(ulong userId);

        /// <summary>
        /// Retrieves all user profiles in the system.
        /// </summary>
        /// <returns>Collection of all <c>UserProfile</c>s in the system.</returns>
        Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync();

        /// <summary>
        /// Retrieves a user profile using the supplied id.
        /// </summary>
        /// <param name="userId">Id of the user profile to retrieve.</param>
        /// <returns>The <c>UserProfile</c> if it exists, otherwise returns null.</returns>
        Task<UserProfile?> GetUserProfileByIdAsync(ulong userId);

        /// <summary>
        /// Retrieves a user profile using the supplied name.
        /// </summary>
        /// <param name="userName">The user name of the profile to retrieve.</param>
        /// <returns>The <c>UserProfile</c> if it exists, otherwise returns null.</returns>
        Task<UserProfile?> GetUserProfileByNameAsync(string userName);

        /// <summary>
        /// Adds a new user profile.
        /// </summary>
        /// <param name="userProfile">The user profile to add.</param>
        /// <returns>The <c>UserProfile</c> that was added (null if none was added).</returns>
        Task<UserProfile?> AddUserProfileAsync(UserProfileForCreation userProfile);

        /// <summary>
        /// Retrieves the screeches in the system.
        /// </summary>
        /// <returns>Collection of all screeches in the system.</returns>
        Task<IEnumerable<Screech>> GetAllScreechesAsync();

        /// <summary>
        /// Retrieves a screech using the supplied Id.
        /// </summary>
        /// <param name="screechId">The Id of the screech to retrieve.</param>
        /// <returns>The <c>Screech</c> if it exists, otherwise returns null.</returns>
        Task<Screech?> GetScreechByIdAsync(ulong screechId);

        /// <summary>
        /// Retrieves a screech made by a user.
        /// </summary>
        /// <param name="creatorId">The creator Id associated with the screech.</param>
        /// <<param name="screechId">The identifier of the screech to retrieve.</param>
        /// <returns>The <c>Screech</c> if it exists, otherwise returns null.</returns>
        Task<Screech?> GetScreechByUserAsync(ulong creatorId, ulong screechId);

        /// <summary>
        /// Adds a new screech.
        /// </summary>
        /// <param name="creatorId">The Id of the user creating the screech.</param>
        /// <param name="screech">The screech to add.</param>
        /// <returns>The <c>Screech</c> that was added (null if none was added).</returns>
        Task<Screech?> AddScreechAsync(ulong creatorId, ScreechForCreation screech);
    }
}
