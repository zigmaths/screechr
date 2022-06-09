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
    /// Contains tests that validate the behaviour of the <c>ScreechController</c>.
    /// </summary>
    public class ScreechControllerTests
    {
        /// <summary>
        /// Tests that GetScreech() returns the correct status code when there is no screech
        /// for the screechId that was provided by the client.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GetScreech_NotFound()
        {
            ulong screechId = 100;  // does not exist
            Screech nullScreech = null;

            // create fake user data repository
            var userDataRepo = A.Fake<IUserDataRepository>();
            A.CallTo(() => userDataRepo.GetScreechByIdAsync(screechId)).Returns(Task.FromResult(nullScreech));

            // create fake logger
            var logger = A.Fake<ILogger<ScreechController>>();

            // create instance of controller
            var controller = new ScreechController(userDataRepo, logger);
            // call into controller
            var actionResult = await controller.GetScreech(screechId);

            // validate response from the controller
            Assert.IsType<NotFoundResult>(actionResult.Result);
            var result = actionResult.Result as NotFoundResult;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}
