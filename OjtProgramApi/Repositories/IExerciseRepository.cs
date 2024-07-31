using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public interface IExerciseRepository : IRepositoryBase<Exercise>
    {
        Task<List<UserExerciseDTO>> GetExercisesWithGradesByUserId(int userID);
        
        Task<bool> CheckExerciseByUserIdAndCourseId(int userId, int courseId, int exerciseId);
    }
}
