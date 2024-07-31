using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public interface ICourseRepository : IRepositoryBase<Course>
    {
       Task<List<UserCourseDTO>> GetCoursesByUserId(int userId);
        
        Task<List<GetStudentsInCourseResponseDTO>> GetStudentsInCourse(int courseID,  string ecbKey);

        Task<bool> BanStudentFromCourse(int courseID, int studentID);

        Task<bool> IsInstructorTeachingCourse(int courseID, int instructorID);
    }
}
