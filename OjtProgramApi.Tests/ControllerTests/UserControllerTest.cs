using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using OjtProgramApi.Controllers;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;
using OjtProgramApi.Util;
using Xunit;

namespace OjtProgramApi.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockConfig = new Mock<IConfiguration>();
            
            _mockRepo.Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.UpdateEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.DeleteEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var loggerMock = new Mock<ILogger<UserController>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<UserController>))).Returns(loggerMock.Object);

            var claims = new List<Claim>
            {
                new Claim("sid", "1")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(principal);
            httpContextMock.Setup(c => c.RequestServices).Returns(serviceProviderMock.Object);

            _mockConfig.Setup(x => x.GetSection("Encyption:ECBSecretKey").Value).Returns("Hello");
            _mockConfig.Setup(x => x.GetSection("Encyption:CBCSecretKey").Value).Returns("HelloCBC");
            _mockConfig.Setup(x => x.GetSection("Encyption:CBCSalt").Value).Returns("Salt");
            _mockConfig.Setup(x => x.GetSection("LoginSettings:MaxLoginFailCount").Value).Returns("3");

            _controller = new UserController(_mockRepo.Object, _mockConfig.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        [Fact]
        public async Task GetAllUsers_ReturnsOk_WhenUsersFound()
        {
            // Arrange
            var users = new List<GetUserResponseDTO> { new GetUserResponseDTO() };
            _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ReturnsAsync(users);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<GetUserResponseDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsNotFound_WhenNoUsersFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ReturnsAsync((List<GetUserResponseDTO>)null);

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetAllUsers();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task CreateUser_ReturnsFail_WhenUserAlreadyExists()
        // {
        //     // Arrange
        //     var userRequest = new UserRequestDTO();
        //     var existingUsers = new List<GetUserResponseDTO> { new GetUserResponseDTO() };
        //     _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ReturnsAsync(existingUsers);

        //     // Act
        //     var result = await _controller.CreateUser(userRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        // }

        // [Fact]
        // public async Task CreateUser_ReturnsSuccess_WhenUserCreated()
        // {
        //     // Arrange
        //     var userRequest = new UserRequestDTO();
        //     _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ReturnsAsync((List<GetUserResponseDTO>)null);

        //     // Act
        //     var result = await _controller.CreateUser(userRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task CreateUser_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var userRequest = new UserRequestDTO();
            _mockRepo.Setup(repo => repo.User.GetAllUsers(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.CreateUser(userRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Login_ReturnsFail_WhenUserNotFound()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO();
            _mockRepo.Setup(repo => repo.User.GetUserByIdentifierAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.Equal("fail", returnValue.status);
        }

        [Fact]
        public async Task Login_ReturnsFail_WhenAccountLocked()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO();
            var user = new User { is_lock = true };
            _mockRepo.Setup(repo => repo.User.GetUserByIdentifierAsync(It.IsAny<string>())).ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
            Assert.Equal("fail", returnValue.status);
        }

        // [Fact]
        // public async Task Login_ReturnsSuccess_WhenLoginSuccessful()
        // {
        //     // Arrange
        //     var loginRequest = new LoginRequestDTO();
        //     var user = new User { is_lock = false, password = "hashedPassword", salt = "salt" };
        //     _mockRepo.Setup(repo => repo.User.GetUserByIdentifierAsync(It.IsAny<string>())).ReturnsAsync(user);
        //     _mockConfig.Setup(config => config["LoginSettings:MaxLoginFailCount"]).Returns("5");

        //     // Act
        //     var result = await _controller.Login(loginRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        // [Fact]
        // public async Task Login_ReturnsFail_WhenPasswordIncorrect()
        // {
        //     // Arrange
        //     var loginRequest = new LoginRequestDTO();
        //     var user = new User { is_lock = false, password = "hashedPassword", salt = "salt", login_fail_count = 0 };
        //     _mockRepo.Setup(repo => repo.User.GetUserByIdentifierAsync(It.IsAny<string>())).ReturnsAsync(user);
        //     _mockConfig.Setup(config => config["LoginSettings:MaxLoginFailCount"]).Returns("5");

        //     // Act
        //     var result = await _controller.Login(loginRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<LoginResponseDTO>(okResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        // }

        [Fact]
        public async Task Login_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var loginRequest = new LoginRequestDTO();
            _mockRepo.Setup(repo => repo.User.GetUserByIdentifierAsync(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Login(loginRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task UpdateUser_ReturnsFail_WhenUserNotFound()
        // {
        //     // Arrange
        //     var updateRequest = new UpdateUserRequestDTO();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync((User)null);

        //     // Act
        //     var result = await _controller.UpdateUser(updateRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        // }

        // [Fact]
        // public async Task UpdateUser_ReturnsSuccess_WhenUserUpdated()
        // {
        //     // Arrange
        //     var updateRequest = new UpdateUserRequestDTO();
        //     var user = new User();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync(user);

        //     // Act
        //     var result = await _controller.UpdateUser(updateRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task UpdateUser_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var updateRequest = new UpdateUserRequestDTO();
            _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UpdateUser(updateRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task UnlockUser_ReturnsFail_WhenUserNotFound()
        // {
        //     // Arrange
        //     var unlockRequest = new UnlockUserRequestDTO();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync((User)null);

        //     // Act
        //     var result = await _controller.UnlockUser(unlockRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        // }

        // [Fact]
        // public async Task UnlockUser_ReturnsSuccess_WhenUserUnlocked()
        // {
        //     // Arrange
        //     var unlockRequest = new UnlockUserRequestDTO();
        //     var user = new User();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync(user);

        //     // Act
        //     var result = await _controller.UnlockUser(unlockRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task UnlockUser_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var unlockRequest = new UnlockUserRequestDTO();
            _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.UnlockUser(unlockRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task DeleteUser_ReturnsFail_WhenUserNotFound()
        // {
        //     // Arrange
        //     var deleteRequest = new DeleteUserRequestDTO();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync((User)null);

        //     // Act
        //     var result = await _controller.DeleteUser(deleteRequest);

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(notFoundResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        // }

        // [Fact]
        // public async Task DeleteUser_ReturnsSuccess_WhenUserDeleted()
        // {
        //     // Arrange
        //     var deleteRequest = new DeleteUserRequestDTO();
        //     var user = new User();
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync(user);

        //     // Act
        //     var result = await _controller.DeleteUser(deleteRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task DeleteUser_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var deleteRequest = new DeleteUserRequestDTO();
            _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.DeleteUser(deleteRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}