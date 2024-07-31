using Microsoft.EntityFrameworkCore;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Util;

namespace OjtProgramApi.Repositories
{
    public class GradeRepository : RepositoryBase<Grade>, IGradeRepository
    {
        public GradeRepository(AppDB context)
            : base(context) { }
         public async Task<string?> GetGradeLetterByExerciseIdAndStudentId(int exerciseID, int studentID)
    {
        var grade = await _context.Grades
            .Where(g => g.ExerciseID == exerciseID && g.StudentID == studentID)
            .Select(g => g.GradeValue)
            .FirstOrDefaultAsync();

        return grade != 0 ? GlobalFunction.ConvertToLetterGrade(grade) : null;
    }
    //     public async Task<List<UserGradeDTO>> GetGradesWithLetterByUserId(int userID)
    //     {
    //         var grades = await (
    //             from g in _context.Grades
    //             join e in _context.Exercises on g.ExerciseID equals e.ExerciseID
    //             where g.StudentID == userID
    //             select new UserGradeDTO
    //             {
    //                 GradeID = g.GradeID,
    //                 ExerciseID = g.ExerciseID,
    //                 ExerciseTitle = e.Title,
    //                 Grade = GlobalFunction.ConvertToLetterGrade(g.GradeValue),
                  
    //             }
    //         ).ToListAsync();

    //         return grades;
    //     }
    // }
    }}
