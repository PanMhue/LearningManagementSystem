using Microsoft.EntityFrameworkCore;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Util;   

namespace OjtProgramApi.Repositories;

public class ExerciseRepository : RepositoryBase<Exercise>, IExerciseRepository
{
    public ExerciseRepository(AppDB context)
        : base(context) { }

    public async Task<List<UserExerciseDTO>> GetExercisesWithGradesByUserId(int userID)
    {
        var exercises = await (
            from ca in _context.CourseAssignments
            join e in _context.Exercises on ca.CourseID equals e.CourseID
            join u in _context.Users on ca.UserID equals u.userID
            join g in _context.Grades on e.ExerciseID equals g.ExerciseID 
            
            where ca.UserID == userID
            select new UserExerciseDTO
            {
                ExerciseID = e.ExerciseID,
                CourseID = e.CourseID,
                Title = e.Title,
                Description = e.Description,
                 Grade =  GlobalFunction.ConvertToLetterGrade(g.GradeValue) 
            // Add GradeValue to the DTO
                 
            }).ToListAsync();

        return exercises;
    }

      public async Task<bool> CheckExerciseByUserIdAndCourseId(int userId, int courseId, int exerciseId)
    {
        var exercise = await (
            from ca in _context.CourseAssignments
            join e in _context.Exercises on ca.CourseID equals e.CourseID
            where ca.UserID == userId && ca.CourseID == courseId && e.ExerciseID == exerciseId
            select e
        ).FirstOrDefaultAsync();
        
        
        if (exercise == null)
        {
            return false;
        }

        return true;
    }
}


