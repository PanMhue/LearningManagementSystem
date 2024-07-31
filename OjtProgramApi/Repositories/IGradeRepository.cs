using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OjtProgramApi.Repositories
{
    public interface IGradeRepository : IRepositoryBase<Grade>
    {
         Task<string?> GetGradeLetterByExerciseIdAndStudentId(int exerciseID, int studentID);
    }
}
