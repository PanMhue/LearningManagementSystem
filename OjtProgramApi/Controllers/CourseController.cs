//assign users to course(admin only)

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OjtProgramApi.Controllers;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;

[ApiController]
public class CourseController : BaseController<CourseController>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;

    public CourseController(IRepositoryWrapper repositoryWrapper, IConfiguration configuration)
    {
        _repositoryWrapper = repositoryWrapper;
        _configuration = configuration;
    }

    [HttpGet("ViewMyCourses", Name = "ViewMyCourses")]
    [Authorize] //students & instructor
    public async Task<ActionResult<List<UserCourseDTO>>> ViewMyCourses()
    {
        Logger?.LogInformation("ViewMyCourses method called");

        try
        {
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
            // Get the user ID from the JWT token
            int userID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");

            if (userID == 0)
            {
                await _repositoryWrapper.EventLog.ErrorEventLog(
                    "CourseController",
                    "ViewMyCourses",
                    "User ID not found in token",
                    "Unauthorized access attempt"
                );
                return Unauthorized("User ID not found in token");
            }

            var userCourses = await _repositoryWrapper.Course.GetCoursesByUserId(userID);

            if (userCourses == null || !userCourses.Any())
            {
                return NotFound("No courses found for the user.");
            }

            await _repositoryWrapper.EventLog.UpdateEventLog(
                "CourseController",
                "ViewMyCourses",
                $"User ID: {userID} viewed their courses."
            );
            return Ok(userCourses);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in ViewMyCourses method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "CourseController",
                "ViewMyCourses",
                "Error occurred while fetching courses",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("CreateCourse", Name = "CreateCourse")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> CreateCourse(CreateCourseDTO courseDto)
    {
        Logger?.LogInformation("CreateCourse method called");

        try
        {
            var newCourse = new Course
            {
                CourseName = courseDto.CourseName,
                Description = courseDto.Description
            };

            _repositoryWrapper.Course.Create(newCourse);
            await _repositoryWrapper.Course.Save();
            await _repositoryWrapper.EventLog.InsertEventLog(
                "CourseController",
                "CreateCourse",
                $"Course created: {newCourse.CourseName}"
            );

            return Ok(
                new CreateUserResponseDTO
                {
                    status = "success",
                    message = "Course created successfully"
                }
            );
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in CreateCourse method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "CourseController",
                "CreateCourses",
                "Error occurred while creating course",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("AssignCourse", Name = "AssignCourse")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> AssignCourse(
        AssignCourseDTO courseassignment
    )
    {
        Logger?.LogInformation("AssignCourse method called");

        try
        {
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
            int userId = int.Parse(Encryption.AES_Decrypt_ECB_128(courseassignment.UserID, ecbKey));
            var course = await _repositoryWrapper.Course.FindByID(courseassignment.CourseID);
            var user = await _repositoryWrapper.User.FindByID(userId);

            if (course == null)
            {
                return BadRequest(
                    new CreateUserResponseDTO { status = "fail", message = "Course not found" }
                );
            }
            if (user == null)
            {
                return BadRequest(
                    new CreateUserResponseDTO { status = "fail", message = "User not found" }
                );
            }

            var CourseAssignment = new CourseAssignment
            {
                CourseID = courseassignment.CourseID,
                UserID = userId,
                
            };

            _repositoryWrapper.CourseAssignment.Create(CourseAssignment);
            await _repositoryWrapper.CourseAssignment.Save();
            await _repositoryWrapper.EventLog.InsertEventLog(
                "CourseController",
                "AssignCourse",
                $"Assigned User ID: {CourseAssignment.UserID} to Course ID: {CourseAssignment.CourseID}"
            );

            return Ok(
                new CreateUserResponseDTO
                {
                    status = "success",
                    message = "User assigned to course successfully"
                }
            );
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in AssignCourse method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "CourseController",
                "AssignCourse",
                "Error occurred while assigning user to course",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("GetStudentsInCourse")]
    [Authorize(Policy = "InstructorPolicy")] // Assuming instructors are allowed to view students
    public async Task<ActionResult<List<GetStudentsInCourseResponseDTO>>> GetStudentsInCourse(
        GetStudentsRequestDTO courseID
    )
    {
        Logger?.LogInformation("GetStudentsInCourse method called");

        string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

        try
        {
            int userID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0"); // Get the user ID from the token

            // Check if the instructor is teaching the course
            bool isInstructorTeachingCourse =
                await _repositoryWrapper.Course.IsInstructorTeachingCourse(
                    courseID.CourseID,
                    userID
                );

            if (!isInstructorTeachingCourse)
            {
                return Forbid();
            }

            var students = await _repositoryWrapper.Course.GetStudentsInCourse(
                courseID.CourseID,
                ecbKey
            );

            if (students == null || students.Count == 0)
            {
                return NotFound(
                    new { status = "fail", message = "No students found for the course" }
                );
            }
            await _repositoryWrapper.EventLog.UpdateEventLog(
                "CourseController",
                "GetStudentsInCourse",
                $"Instructor ID: {userID} viewed students in course ID: {courseID.CourseID}"
            );
            return Ok(students);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in GetStudentsInCourse method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "CourseController",
                "GetStudentsInCourse",
                "Failed to get students in course",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("BanStudentFromCourse")]
    [Authorize(Policy = "InstructorPolicy")] // Assuming instructors are allowed to ban students
    public async Task<ActionResult> BanStudentFromCourse(BanStudentRequestDTO banStudentRequest)
    {
        Logger?.LogInformation("BanStudentFromCourse method called");

        try
        {
            var encryptedKey = banStudentRequest.StudentID;
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

            bool result = await _repositoryWrapper.Course.BanStudentFromCourse(
                banStudentRequest.CourseID,
                int.Parse(Encryption.AES_Decrypt_ECB_128(banStudentRequest.StudentID, ecbKey))
            );

            if (!result)
            {
                return NotFound(
                    new
                    {
                        status = "fail",
                        message = "Student not found or already banned from the course"
                    }
                );
            }
            await _repositoryWrapper.EventLog.UpdateEventLog(
                "CourseController",
                "BanStudentFromCourse",
                $"Instructor banned student ID: {encryptedKey} from course ID: {banStudentRequest.CourseID}"
            );
            return Ok(new { status = "success", message = "Student banned successfully" });
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in BanStudentFromCourse method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "CourseController",
                "BanStudentFromCourse",
                "Failed to ban student from course",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }
}
