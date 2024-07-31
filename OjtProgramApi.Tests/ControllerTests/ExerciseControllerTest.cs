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
    public class ExerciseControllerTest
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly ExerciseController _controller;

        public ExerciseControllerTest()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockConfig = new Mock<IConfiguration>();
            
            _mockRepo.Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.UpdateEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.DeleteEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var loggerMock = new Mock<ILogger<ExerciseController>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<ExerciseController>))).Returns(loggerMock.Object);

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

            _controller = new ExerciseController(_mockRepo.Object, _mockConfig.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        // [Fact]
        // public async Task ViewMyExercisesWithGrades_ReturnsUnauthorized_WhenUserIdNotFoundInToken()
        // {
        //     // Arrange
        //     var httpContextMock = new Mock<HttpContext>();
        //     httpContextMock.Setup(c => c.User.FindFirst(It.IsAny<string>())).Returns((Claim)null);
        //     _controller.ControllerContext.HttpContext = httpContextMock.Object;

        //     // Act
        //     var result = await _controller.ViewMyExercisesWithGrades();

        //     // Assert
        //     var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        //     Assert.Equal("User ID not found in token", unauthorizedResult.Value);
        // }

        [Fact]
        public async Task ViewMyExercisesWithGrades_ReturnsOk_WhenExercisesFound()
        {
            // Arrange
            var exercises = new List<UserExerciseDTO> { new UserExerciseDTO() };
            _mockRepo.Setup(repo => repo.Exercise.GetExercisesWithGradesByUserId(It.IsAny<int>())).ReturnsAsync(exercises);

            // Act
            var result = await _controller.ViewMyExercisesWithGrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<UserExerciseDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ViewMyExercisesWithGrades_ReturnsNotFound_WhenNoExercisesFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Exercise.GetExercisesWithGradesByUserId(It.IsAny<int>())).ReturnsAsync((List<UserExerciseDTO>)null);

            // Act
            var result = await _controller.ViewMyExercisesWithGrades();

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task ViewMyExercisesWithGrades_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Exercise.GetExercisesWithGradesByUserId(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ViewMyExercisesWithGrades();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task CreateExercise_ReturnsSuccess_WhenExerciseCreated()
        // {
        //     // Arrange
        //     var exerciseDto = new CreateExerciseDTO { Title = "Test Exercise", Description = "Test Description" };

        //     // Act
        //     var result = await _controller.CreateExercise(exerciseDto);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task CreateExercise_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var exerciseDto = new CreateExerciseDTO { Title = "Test Exercise", Description = "Test Description" };
            _mockRepo.Setup(repo => repo.Exercise.Create(It.IsAny<Exercise>())).Throws(new Exception());

            // Act
            var result = await _controller.CreateExercise(exerciseDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task AssignExercise_ReturnsFail_WhenUserOrCourseNotExist()
        // {
        //     // Arrange
        //     var assignExerciseDto = new AssignExerciseDTO { CourseID = 1, ExerciseID = 1, UserID = "encryptedStudentId" };
        //     _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        //     // Act
        //     var result = await _controller.AssignExercise(assignExerciseDto);

        //     // Assert
        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        //     Assert.Equal("Fail to create Exercise due to User (or) Course does not exist.", returnValue.message);
        // }

        // [Fact]
        // public async Task AssignExercise_ReturnsSuccess_WhenExerciseAssigned()
        // {
        //     // Arrange
        //     var assignExerciseDto = new AssignExerciseDTO { CourseID = 1, ExerciseID = 1, UserID = "encryptedStudentId" };
        //     _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        //     // Act
        //     var result = await _controller.AssignExercise(assignExerciseDto);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        //     Assert.Equal("Exercise assigned successfully", returnValue.message);
        // }

        [Fact]
        public async Task AssignExercise_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var assignExerciseDto = new AssignExerciseDTO { CourseID = 1, ExerciseID = 1, UserID = "encryptedStudentId" };
            _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.AssignExercise(assignExerciseDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task MarkExercise_ReturnsFail_WhenUserOrCourseNotExist()
        // {
        //     // Arrange
        //     var markExerciseDto = new MarkExerciseDTO { CourseID = 1, ExerciseID = 1, StudentID = "encryptedStudentId", Grade = 90 };
        //     _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        //     // Act
        //     var result = await _controller.MarkExercise(markExerciseDto);

        //     // Assert
        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        //     Assert.Equal("Fail to mark Exercise due to User (or) Course does not exist.", returnValue.message);
        // }

        // [Fact]
        // public async Task MarkExercise_ReturnsSuccess_WhenExerciseMarked()
        // {
        //     // Arrange
        //     var markExerciseDto = new MarkExerciseDTO { CourseID = 1, ExerciseID = 1, StudentID = "encryptedStudentId", Grade = 90 };
        //     _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        //     // Act
        //     var result = await _controller.MarkExercise(markExerciseDto);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        //     Assert.Equal("Exercise marked successfully", returnValue.message);
        // }

        [Fact]
        public async Task MarkExercise_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var markExerciseDto = new MarkExerciseDTO { CourseID = 1, ExerciseID = 1, StudentID = "encryptedStudentId", Grade = 90 };
            _mockRepo.Setup(repo => repo.Exercise.CheckExerciseByUserIdAndCourseId(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.MarkExercise(markExerciseDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}