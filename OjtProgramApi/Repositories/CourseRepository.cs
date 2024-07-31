using Microsoft.EntityFrameworkCore;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories;

public class CourseRepository : RepositoryBase<Course>, ICourseRepository
{
    public CourseRepository(AppDB context)
        : base(context) { }

    public async Task<List<UserCourseDTO>> GetCoursesByUserId(int userID)
    {
        var courses = await (
            from ca in _context.CourseAssignments
            join c in _context.Courses on ca.CourseID equals c.CourseID
            join u in _context.Users on ca.UserID equals u.userID // Join Users to get RoleID
            join r in _context.Roles on u.RoleID equals r.RoleID // Join Roles to get RoleName
            where ca.UserID == userID
            select new UserCourseDTO
            {
                CourseID = c.CourseID,
                CourseName = c.CourseName,
                Description = c.Description,
                Role = r.RoleName // Get RoleName
            }
        ).ToListAsync();
        return courses;
    }

    public async Task<List<GetStudentsInCourseResponseDTO>> GetStudentsInCourse(
        int CourseID,
        string ecbKey
    )
    {
     var students = await (
            from ca in _context.CourseAssignments
            join u in _context.Users on ca.UserID equals u.userID
            where ca.CourseID == CourseID && u.RoleID == 3
            select new GetStudentsInCourseResponseDTO
            {
                UserID = Encryption.AES_Encrypt_ECB_128(u.userID.ToString(), ecbKey),
                FirstName = u.FirstName,
                LastName = u.LastName,
            }
        ).ToListAsync();

        return students;
    }

    public async Task<bool> IsInstructorTeachingCourse(int courseID, int instructorID)
    {
        var courseAssigment =  await (
            from ca in _context.CourseAssignments
            where ca.CourseID == courseID && ca.UserID == instructorID
            select ca
        ).FirstOrDefaultAsync();
        
        if (courseAssigment == null)
        {
            return false;
        }

        return true;
    }

    public async Task<bool> BanStudentFromCourse(int courseID, int studentID)
    {
        var courseAssignment = await _context.CourseAssignments.FirstOrDefaultAsync(ca =>
            ca.CourseID == courseID && ca.UserID == studentID
        );

        if (courseAssignment == null)
        {
            // No course assignment found
            return false;
        }

        // Remove the student from course assignment
        _context.CourseAssignments.Remove(courseAssignment);
        await _context.SaveChangesAsync();
        return true;
    }
}