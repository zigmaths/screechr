using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using screechr.Controllers;
using screechr.Models;
using screechr.Services;

namespace Screechr.Test
{
    /// <summary>
    /// Contains tests that validate the behaviour of the <c>UserProfileController</c>.
    /// </summary>
    public class UserProfileControllerTests
    {
        /// <summary>
        /// Tests that CreateUserProfile() returns the correct status code when the user name
        /// provided by the client is already taken.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateUserProfile_UserName_Taken()
        {
            UserProfileForCreation userCreateRequest = new UserProfileForCreation
            {
                UserName = "jdoe",
                Password = "abc123",
                FirstName = "John",
                LastName = "Doe"
            };

            // create fake user data repository that returns true (i.e. profile with user name exists)
            var userDataRepo = A.Fake<IUserDataRepository>();
            A.CallTo(() => userDataRepo.UserProfileExistsByNameAsync(userCreateRequest.UserName)).Returns(Task.FromResult(true));

            // create fake logger
            var logger = A.Fake<ILogger<UserProfileController>>();

            // create fake mapper
            var mapper = A.Fake<IMapper>();

            var controller = new UserProfileController(userDataRepo, logger, mapper);
            // call into controller
            var actionResult = await controller.CreateUserProfile(userCreateRequest);

            // validate response from controller
            Assert.IsType<ConflictResult>(actionResult.Result);
            var result = actionResult.Result as ConflictResult;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status409Conflict, result.StatusCode);
        }

        /// <summary>
        /// Tests that GetAllProfiles() returns the correct number of user profiles.
        /// </summary>
        [Fact]
        public async Task GetAllProfiles_Returns_Correct_Number_Of_Profiles()
        {
            int count = 3;
            // create fake user profiles
            var fakeUserProfiles = A.CollectionOfDummy<UserProfile>(count).AsEnumerable();
            var fakeUserProfilesWithoutToken = A.CollectionOfDummy<UserProfileWithoutToken>(count).AsEnumerable();

            // create fake user data repository
            var userDataRepo = A.Fake<IUserDataRepository>();
            A.CallTo(() => userDataRepo.GetAllUserProfilesAsync()).Returns(Task.FromResult(fakeUserProfiles));

            // create fake logger
            var logger = A.Fake<ILogger<UserProfileController>>();

            // create fake mapper that returns fake UserProfileWithoutToken
            var mapper = A.Fake<IMapper>();
            A.CallTo(() => mapper.Map<IEnumerable<UserProfileWithoutToken>>(fakeUserProfiles)).Returns(fakeUserProfilesWithoutToken);

            var controller = new UserProfileController(userDataRepo, logger, mapper);
            // call into controller
            var actionResult = await controller.GetAllUserProfiles();

            // validate that the correct number of profiles is returned.
            var result = actionResult.Result as OkObjectResult;
            Assert.NotNull(result);
            var userProfilesReturned = result.Value as IEnumerable<UserProfileWithoutToken>;
            Assert.NotNull(userProfilesReturned);
            Assert.Equal(count, userProfilesReturned.Count());
        }
    }
}