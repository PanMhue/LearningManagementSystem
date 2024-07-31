using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OjtProgramApi.Controllers;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;
using OjtProgramApi.Util;
using Serilog;

[ApiController]
public class ExerciseController : BaseController<ExerciseController>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;

    public ExerciseController(IRepositoryWrapper repositoryWrapper, IConfiguration configuration)
    {
        _repositoryWrapper = repositoryWrapper;
        _configuration = configuration;
    }

    [HttpPost("CreateExercise", Name = "CreateExercise")]
    [Authorize(Policy = "InstructorPolicy")] 
    public async Task<ActionResult<CreateUserResponseDTO>> CreateExercise(CreateExerciseDTO exerciseDto)
    {
        Logger?.LogInformation("CreateExercise method called");

        try
        {
            var newExercise = new Exercise
            {
                CourseID = exerciseDto.CourseID,
                Title = exerciseDto.Title,
                Description = exerciseDto.Description,
                CreatedBy = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0"),
                CreatedAt = DateTime.Now
            };

            _repositoryWrapper.Exercise.Create(newExercise);
            await _repositoryWrapper.Exercise.Save();
            await _repositoryWrapper.EventLog.InsertEventLog(
                "ExerciseController",
                "CreateExercise",
                $"Exercise created: {exerciseDto.Title}"
            );

            return Ok(new CreateUserResponseDTO
            {
                status = "success",
                message = "Exercise created successfully"
            });
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in CreateExercise method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "ExerciseController",
                "CreateExercise",
                "Failed to Create exercises",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }
    
    [HttpPost("AssignExercise", Name = "AssignExercise")]
    [Authorize(Policy = "InstructorPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> AssignExercise(AssignExerciseDTO assignmentDto)
    {
        Logger?.LogInformation("AssignExercise method called");

        try
        {
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
            int studentId = int.Parse(Encryption.AES_Decrypt_ECB_128(assignmentDto.UserID, ecbKey));
            int courseId = assignmentDto.CourseID;
            int exerciseId = assignmentDto.ExerciseID;

            bool isExerciseExist = await _repositoryWrapper.Exercise.CheckExerciseByUserIdAndCourseId(studentId, courseId, exerciseId);
            
            if (!isExerciseExist)
            {
                return BadRequest(new CreateUserResponseDTO { status = "fail", message = "Fail to create Exercise due to User (or) Course does not exist." });
            }

            ExerciseAssignment assignment = new ExerciseAssignment()
            {
                CourseID = courseId,
                ExerciseID = exerciseId,
                StudentID = studentId,
            };
            
            _repositoryWrapper.ExerciseAssignment.Create(assignment);
            await _repositoryWrapper.ExerciseAssignment.Save();
            await _repositoryWrapper.EventLog.InsertEventLog(
                "ExerciseController",
                "AssignExercise",
                $"Assigned Exercise ID: {assignment.ExerciseID} assigned to Student ID: {assignment.StudentID} in Course ID: {assignment.CourseID}"
            );

            return Ok(new CreateUserResponseDTO
            {
                status = "success",
                message = "Exercise assigned successfully"
            });
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in AssignExercise method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "ExerciseController",
                "AssignExercise",
                "Failed to assign exercise",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

     [HttpPost("MarkExercise", Name = "MarkExercise")]
     [Authorize(Policy = "InstructorPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> MarkExercise(MarkExerciseDTO markExerciseDto)
    {
        Logger?.LogInformation("MarkExercise method called");

        try
        {
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";
            int studentId = int.Parse(Encryption.AES_Decrypt_ECB_128(markExerciseDto.StudentID, ecbKey));
            int courseId = markExerciseDto.CourseID;
            int exerciseId = markExerciseDto.ExerciseID;
            
            bool isExerciseExist = await _repositoryWrapper.Exercise.CheckExerciseByUserIdAndCourseId(studentId, courseId, exerciseId);
            
            if (!isExerciseExist)
            {
                return BadRequest(new CreateUserResponseDTO { status = "fail", message = "Fail to mark Exercise due to User (or) Course does not exist." });
            }

            var grade = new Grade
            {
                ExerciseID = markExerciseDto.ExerciseID,
                StudentID = studentId,
                InstructorID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0"),
                GradeValue = markExerciseDto.Grade,
                GradedAt = DateTime.Now
            };

            _repositoryWrapper.Grade.Create(grade);
            await _repositoryWrapper.Grade.Save();
            await _repositoryWrapper.EventLog.InsertEventLog(
                "ExerciseController",
                "MarkExercise",
                 $"Exercise ID: {markExerciseDto.ExerciseID} marked for Student ID: {markExerciseDto.StudentID}"
            );

            return Ok(new CreateUserResponseDTO
            {
                status = "success",
                message = "Exercise marked successfully"
            });
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in MarkExercise method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "ExerciseController",
                "MarkExercise",
                "Failed to mark exercises",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("ViewMyExercisesWithGrades", Name = "ViewMyExercisesWithGrades")]
    [Authorize (Policy = "StudentPolicy")]
    public async Task<ActionResult<List<UserExerciseDTO>>> ViewMyExercisesWithGrades()
    {
        Logger?.LogInformation("ViewMyExercisesWithGrades method called");

        try
        {
            int userID = int.Parse(HttpContext.User?.FindFirst(x => x.Type == "sid")?.Value ?? "0");
            var exercises = await _repositoryWrapper.Exercise.GetExercisesWithGradesByUserId(userID);

            if (exercises == null || !exercises.Any())
            {
                return NotFound();
            }

            await _repositoryWrapper.EventLog.UpdateEventLog(
                "ExerciseController",
                "ViewMyExercises",
                $"User ID: {userID} viewed their exercises."
            );

            return Ok(exercises);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in ViewMyExercises method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "ExerciseController",
                "ViewMyExercises",
                "Failed to view exercises",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }
}

//ViewTOdoExercies (students)
//MarkExercise (Instrcutors)
//CreateExercise (Instructors)
//AssignExercise (Instructors)