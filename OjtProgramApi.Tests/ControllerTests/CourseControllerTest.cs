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
    public class CourseControllerTests
    {
        private readonly Mock<IRepositoryWrapper> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            _mockRepo = new Mock<IRepositoryWrapper>();
            _mockConfig = new Mock<IConfiguration>();
            
            _mockRepo.Setup(repo => repo.EventLog.InsertEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.UpdateEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.ErrorEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
            _mockRepo.Setup(repo => repo.EventLog.DeleteEventLog(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            var loggerMock = new Mock<ILogger<CourseController>>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(sp => sp.GetService(typeof(ILogger<CourseController>))).Returns(loggerMock.Object);

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

            _controller = new CourseController(_mockRepo.Object, _mockConfig.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContextMock.Object
                }
            };
        }

        // [Fact]
        // public async Task ViewMyCourses_ReturnsUnauthorized_WhenUserIdNotFoundInToken()
        // {
        //     // Arrange
        //     var httpContextMock = new Mock<HttpContext>();
        //     httpContextMock.Setup(c => c.User.FindFirst(It.IsAny<string>())).Returns((Claim)null);
        //     _controller.ControllerContext.HttpContext = httpContextMock.Object;

        //     // Act
        //     var result = await _controller.ViewMyCourses();

        //     // Assert
        //     var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        //     Assert.Equal("User ID not found in token", unauthorizedResult.Value);
        // }

        [Fact]
        public async Task ViewMyCourses_ReturnsOk_WhenCoursesFound()
        {
            // Arrange
            var courses = new List<UserCourseDTO> { new UserCourseDTO() };
            _mockRepo.Setup(repo => repo.Course.GetCoursesByUserId(It.IsAny<int>())).ReturnsAsync(courses);

            // Act
            var result = await _controller.ViewMyCourses();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<UserCourseDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task ViewMyCourses_ReturnsNotFound_WhenNoCoursesFound()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Course.GetCoursesByUserId(It.IsAny<int>())).ReturnsAsync((List<UserCourseDTO>)null);

            // Act
            var result = await _controller.ViewMyCourses();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No courses found for the user.", notFoundResult.Value);
        }

        [Fact]
        public async Task ViewMyCourses_ReturnsInternalServerError_OnException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.Course.GetCoursesByUserId(It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ViewMyCourses();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task CreateCourse_ReturnsSuccess_WhenCourseCreated()
        // {
        //     // Arrange
        //     var courseDto = new CreateCourseDTO { CourseName = "Test Course", Description = "Test Description" };

        //     // Act
        //     var result = await _controller.CreateCourse(courseDto);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        // }

        [Fact]
        public async Task CreateCourse_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var courseDto = new CreateCourseDTO { CourseName = "Test Course", Description = "Test Description" };
            _mockRepo.Setup(repo => repo.Course.Create(It.IsAny<Course>())).Throws(new Exception());

            // Act
            var result = await _controller.CreateCourse(courseDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task AssignCourse_ReturnsFail_WhenCourseNotFound()
        // {
        //     // Arrange
        //     var courseAssignment = new AssignCourseDTO { CourseID = 1, UserID = "encryptedUserId" };
        //     _mockRepo.Setup(repo => repo.Course.FindByID(It.IsAny<int>())).ReturnsAsync((Course)null);

        //     // Act
        //     var result = await _controller.AssignCourse(courseAssignment);

        //     // Assert
        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        //     Assert.Equal("Course not found", returnValue.message);
        // }

        // [Fact]
        // public async Task AssignCourse_ReturnsFail_WhenUserNotFound()
        // {
        //     // Arrange
        //     var courseAssignment = new AssignCourseDTO { CourseID = 1, UserID = "encryptedUserId" };
        //     _mockRepo.Setup(repo => repo.Course.FindByID(It.IsAny<int>())).ReturnsAsync(new Course());
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync((User)null);

        //     // Act
        //     var result = await _controller.AssignCourse(courseAssignment);

        //     // Assert
        //     var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(badRequestResult.Value);
        //     Assert.Equal("fail", returnValue.status);
        //     Assert.Equal("User not found", returnValue.message);
        // }

        // [Fact]
        // public async Task AssignCourse_ReturnsSuccess_WhenUserAssignedToCourse()
        // {
        //     // Arrange
        //     var courseAssignment = new AssignCourseDTO { CourseID = 1, UserID = "encryptedUserId" };
        //     _mockRepo.Setup(repo => repo.Course.FindByID(It.IsAny<int>())).ReturnsAsync(new Course());
        //     _mockRepo.Setup(repo => repo.User.FindByID(It.IsAny<int>())).ReturnsAsync(new User());

        //     // Act
        //     var result = await _controller.AssignCourse(courseAssignment);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<CreateUserResponseDTO>(okResult.Value);
        //     Assert.Equal("success", returnValue.status);
        //     Assert.Equal("User assigned to course successfully", returnValue.message);
        // }

        [Fact]
        public async Task AssignCourse_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var courseAssignment = new AssignCourseDTO { CourseID = 1, UserID = "encryptedUserId" };
            _mockRepo.Setup(repo => repo.Course.FindByID(It.IsAny<int>())).Throws(new Exception());

            // Act
            var result = await _controller.AssignCourse(courseAssignment);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetStudentsInCourse_ReturnsForbid_WhenInstructorNotTeachingCourse()
        {
            // Arrange
            var courseID = new GetStudentsRequestDTO { CourseID = 1 };
            _mockRepo.Setup(repo => repo.Course.IsInstructorTeachingCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

            // Act
            var result = await _controller.GetStudentsInCourse(courseID);

            // Assert
            Assert.IsType<ForbidResult>(result.Result);
        }

        [Fact]
        public async Task GetStudentsInCourse_ReturnsOk_WhenStudentsFound()
        {
            // Arrange
            var courseID = new GetStudentsRequestDTO { CourseID = 1 };
            var students = new List<GetStudentsInCourseResponseDTO> { new GetStudentsInCourseResponseDTO() };
            _mockRepo.Setup(repo => repo.Course.IsInstructorTeachingCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _mockRepo.Setup(repo => repo.Course.GetStudentsInCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(students);

            // Act
            var result = await _controller.GetStudentsInCourse(courseID);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<GetStudentsInCourseResponseDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        // [Fact]
        // public async Task GetStudentsInCourse_ReturnsNotFound_WhenNoStudentsFound()
        // {
        //     // Arrange
        //     var courseID = new GetStudentsRequestDTO { CourseID = 1 };
        //     _mockRepo.Setup(repo => repo.Course.IsInstructorTeachingCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
        //     _mockRepo.Setup(repo => repo.Course.GetStudentsInCourse(It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((List<GetStudentsInCourseResponseDTO>)null);

        //     // Act
        //     var result = await _controller.GetStudentsInCourse(courseID);

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        //     var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
        //     Assert.Equal("fail", returnValue["status"]);
        //     Assert.Equal("No students found for the course", returnValue["message"]);
        // }

        [Fact]
        public async Task GetStudentsInCourse_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var courseID = new GetStudentsRequestDTO { CourseID = 1 };
            _mockRepo.Setup(repo => repo.Course.IsInstructorTeachingCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
            _mockRepo.Setup(repo => repo.Course.GetStudentsInCourse(It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.GetStudentsInCourse(courseID);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        // [Fact]
        // public async Task BanStudentFromCourse_ReturnsNotFound_WhenStudentNotFoundOrAlreadyBanned()
        // {
        //     // Arrange
        //     var banStudentRequest = new BanStudentRequestDTO { CourseID = 1, StudentID = "encryptedStudentId" };
        //     _mockRepo.Setup(repo => repo.Course.BanStudentFromCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        //     // Act
        //     var result = await _controller.BanStudentFromCourse(banStudentRequest);

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //     var returnValue = Assert.IsType<Dictionary<string, string>>(notFoundResult.Value);
        //     Assert.Equal("fail", returnValue["status"]);
        //     Assert.Equal("Student not found or already banned from the course", returnValue["message"]);
        // }

        // [Fact]
        // public async Task BanStudentFromCourse_ReturnsSuccess_WhenStudentBannedSuccessfully()
        // {
        //     // Arrange
        //     var banStudentRequest = new BanStudentRequestDTO { CourseID = 1, StudentID = "encryptedStudentId" };
        //     _mockRepo.Setup(repo => repo.Course.BanStudentFromCourse(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);

        //     // Act
        //     var result = await _controller.BanStudentFromCourse(banStudentRequest);

        //     // Assert
        //     var okResult = Assert.IsType<OkObjectResult>(result);
        //     var returnValue = Assert.IsType<Dictionary<string, string>>(okResult.Value);
        //     Assert.Equal("success", returnValue["status"]);
        //     Assert.Equal("Student banned successfully", returnValue["message"]);
        // }

        [Fact]
        public async Task BanStudentFromCourse_ReturnsInternalServerError_OnException()
        {
            // Arrange
            var banStudentRequest = new BanStudentRequestDTO { CourseID = 1, StudentID = "encryptedStudentId" };
            _mockRepo.Setup(repo => repo.Course.BanStudentFromCourse(It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.BanStudentFromCourse(banStudentRequest);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}