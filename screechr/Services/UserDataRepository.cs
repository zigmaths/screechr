using screechr.Models;
using System.Collections.Concurrent;

namespace screechr.Services
{
    /// <summary>
    /// Represents the database where all user data is stored.
    /// </summary>
    public class UserDataRepository : IUserDataRepository
    {
        private static ConcurrentDictionary<ulong, UserProfile> _userProfiles =
              new ConcurrentDictionary<ulong, UserProfile>();
        private static ConcurrentDictionary<ulong, Screech> _screeches =
              new ConcurrentDictionary<ulong, Screech>();
        private ulong _userIdSeed = 0;  // TODO: for demo purposes; need to look at better ways of generating the user Id
        private ulong _screechIdSeed = 0;  // TODO: for demo purposes; need to look at better ways of generating the screech Id
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor method.
        /// </summary>
        /// <param name="configuration">The configuration settings for the application.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <c>configuration</c> is null.</exception>
        public UserDataRepository(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));

            // for demo purposes only
            var t = SeedRepository();
            t.Wait();
        }

        #region IUserDataRepository Members

        /// <summary>
        /// Checks whether a profile exists for the supplied user name.
        /// </summary>
        /// <param name="userName">The user name to check the profile for.</param>
        /// <returns><c>true</c> if a profile for the user name exists, false otherwise.</returns>
        public async Task<bool> UserProfileExistsByNameAsync(string userName)
        {
            // simulate async call to database
            return await Task.Run(() => _userProfiles.Values.Where(
                x => string.Compare(x.UserName, userName, StringComparison.OrdinalIgnoreCase) == 0).Count() > 0);
        }

        /// <summary>
        /// Checks whether a profile exists for the supplied Id.
        /// </summary>
        /// <param name="userId">The Id to check the profile for.</param>
        /// <returns><c>true</c> if a profile for the Id exists, false otherwise.</returns>
        public async Task<bool> UserProfileExistsByIdAsync(ulong userId)
        {
            // simulate async call to database
            return await Task.Run(() => _userProfiles.ContainsKey(userId));
        }

        /// <summary>
        /// Retrieves all user profiles in the system.
        /// </summary>
        /// <returns>Collection of all <c>UserProfile</c>s in the system.</returns>
        public async Task<IEnumerable<UserProfile>> GetAllUserProfilesAsync()
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                return _userProfiles.Values;
            });
        }

        /// <summary>
        /// Retrieves a user profile using the supplied id.
        /// </summary>
        /// <param name="userId">Id of the profile to retrieve.</param>
        /// <returns>The <c>UserProfile</c> if it exists, otherwise returns null.</returns>
        public async Task<UserProfile?> GetUserProfileByIdAsync(ulong userId)
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                UserProfile profile;
                _userProfiles.TryGetValue(userId, out profile);
                return profile;
            });
        }

        /// <summary>
        /// Retrieves a user profile using the supplied name.
        /// </summary>
        /// <param name="userName">The user name of the profile to retrieve.</param>
        /// <returns>The <c>UserProfile</c> if it exists, otherwise returns null.</returns>
        public async Task<UserProfile?> GetUserProfileByNameAsync(string userName)
        {
            // simulate async call to database
            return await Task.Run(() => _userProfiles.Values.Where(
                x => string.Compare(x.UserName, userName, StringComparison.OrdinalIgnoreCase) == 0).FirstOrDefault());
        }

        /// <summary>
        /// Adds a new user profile.
        /// </summary>
        /// <param name="userProfile">The user profile to add.</param>
        /// <returns>The <c>UserProfile</c> that was added (null if none was added).</returns>
        public async Task<UserProfile?> AddUserProfileAsync(UserProfileForCreation userProfile)
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                UserProfile finalUserProfile = new UserProfile(_configuration)
                {
                    Id = ++_userIdSeed, // TODO: for demo purposes only; need to look at better ways of generating the Id
                    UserName = userProfile.UserName,
                    Password = userProfile.Password,
                    FirstName = userProfile.FirstName,
                    LastName = userProfile.LastName,
                    ProfileImage = userProfile.ProfileImage,
                    DateCreated = DateTime.UtcNow.ToString("o")
                };

                if (_userProfiles.TryAdd(finalUserProfile.Id, finalUserProfile))
                    return finalUserProfile;
                //throw new Exception($"Failed to add user profile for { userProfile.LastName }, { userProfile.FirstName }"); // TODO: need to create an exception class for this error condition

                return null;
            });
        }

        /// <summary>
        /// Retrieves a screech made by a user.
        /// </summary>
        /// <param name="creatorId">The creator Id associated with the screech.</param>
        /// <<param name="screechId">The identifier of the screech to retrieve.</param>
        /// <returns>The <c>Screech</c> if it exists, otherwise returns null.</returns>
        public async Task<Screech?> GetScreechByUserAsync(ulong creatorId, ulong screechId)
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                return _screeches.Values.Where(s => s.Id == screechId && s.CreatorId == creatorId).FirstOrDefault();
            });
        }

        /// <summary>
        /// Retrieves the screeches in the system.
        /// </summary>
        /// <returns>Collection of all screeches in the system.</returns>
        public async Task<IEnumerable<Screech>> GetAllScreechesAsync()
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                return _screeches.Values;
            });
        }

        /// <summary>
        /// Retrieves a screech using the supplied Id.
        /// </summary>
        /// <param name="screechId">The Id of the screech to retrieve.</param>
        /// <returns>The <c>Screech</c> if it exists, otherwise returns null.</returns>
        public async Task<Screech?> GetScreechByIdAsync(ulong screechId)
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                // simulate async call to database
                Screech screech;
                _screeches.TryGetValue(screechId, out screech);
                return screech;
            });
        }

        /// <summary>
        /// Adds a new screech.
        /// </summary>
        /// <param name="creatorId">The Id of the user creating the screech.</param>
        /// <param name="screech">The screech to add.</param>
        /// <returns>The <c>Screech</c> that was added (null if none was added).</returns>
        public async Task<Screech?> AddScreechAsync(ulong creatorId, ScreechForCreation screech)
        {
            // simulate async call to database
            return await Task.Run(() =>
            {
                Screech finalScreech = new Screech()
                {
                    Id = ++_screechIdSeed, // TODO: for demo purposes only; need to look at better ways of generating the Id
                    Content = screech.Content,
                    CreatorId = creatorId,
                    DateCreated = DateTime.UtcNow.ToString("o")
                };

                if (_screeches.TryAdd(finalScreech.Id, finalScreech))
                    return finalScreech;

                return null;
            });
        }

        #endregion IUserDataRepository Members

        /// <summary>
        /// Populate the repo with initial data (for demo purposes).
        /// </summary>
        /// <returns></returns>
        private async Task SeedRepository()
        {
            await AddUserProfileAsync(new UserProfileForCreation
            {
                UserName = "iamyourfather",
                Password = "Password1",
                LastName = "Skywalker",
                FirstName = "Anakin",
                ProfileImage = new Uri("https://lumiere-a.akamaihd.net/v1/images/Darth-Vader_6bda9114.jpeg?region=0%2C23%2C1400%2C785&width=600")
            });
            var userProfile = await GetUserProfileByNameAsync("iamyourfather");
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Anakin's first screech."
            });
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Anakin's second screech."
            });

            await AddUserProfileAsync(new UserProfileForCreation
            {
                UserName = "chewy",
                Password = "Password2",
                LastName = "Solo",
                FirstName = "Han",
                ProfileImage = new Uri("https://static.wikia.nocookie.net/starwars/images/0/01/Hansoloprofile.jpg/revision/latest?cb=20100129155042")
            });
            userProfile = await GetUserProfileByNameAsync("chewy");
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Han's first screech."
            });
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Han's second screech."
            });

            await AddUserProfileAsync(new UserProfileForCreation
            {
                UserName = "merc44",
                Password = "Password3",
                LastName = "Hamilton",
                FirstName = "Lewis",
                ProfileImage = new Uri("https://www.topgear.com/sites/default/files/images/news-article/carousel/2020/11/cdfa20172ebb83b9e2191625850c1f63/m252101.jpg?w=211&h=119")
            });
            userProfile = await GetUserProfileByNameAsync("merc44");
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Lewis' first screech."
            });
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Lewis' second screech."
            });

            await AddUserProfileAsync(new UserProfileForCreation
            {
                UserName = "redbull33",
                Password = "Password4",
                LastName = "Verstappen",
                FirstName = "Max",
                ProfileImage = new Uri("https://static01.nyt.com/images/2022/01/10/sports/10sp-dhabi-longer-inyt/merlin_199158756_22a0ff44-86fb-4536-a052-0b0e020e816e-jumbo.jpg?quality=75&auto=webp")
            });
            userProfile = await GetUserProfileByNameAsync("redbull33");
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Max's first screech."
            });
            await AddScreechAsync(userProfile.Id, new ScreechForCreation()
            {
                Content = "This is Max's second screech."
            });
        }
    }
}
