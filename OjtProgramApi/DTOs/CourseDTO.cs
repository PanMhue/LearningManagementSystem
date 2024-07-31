namespace OjtProgramApi.DTO;
public class CreateCourseDTO
{
    public string CourseName { get; set; }
    public string Description { get; set; }
}

public class CourseResponseDTO
{
    public int CourseID { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }
}
public class AssignCourseDTO
{
    public int CourseID { get; set; }
    public string UserID { get; set; }
}

public class UserCourseDTO
{
    public int CourseID { get; set; }
    public string CourseName { get; set; }
    public string Description { get; set; }

    public string Role { get; set; }
    
}

 public class GetStudentsInCourseResponseDTO
    {
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

public class BanStudentRequestDTO
{
    public int CourseID { get; set; }
    public string StudentID { get; set; }
}


public class GetStudentsRequestDTO
{
    public int CourseID { get; set; }
    
}
