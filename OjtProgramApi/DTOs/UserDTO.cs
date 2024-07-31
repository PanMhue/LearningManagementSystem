namespace OjtProgramApi.DTO
{
    public class UserRequestDTO
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public int RoleID { get; set; }
    }

    public class CreateUserResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
    }

    public class LoginRequestDTO
    {
        public string UserNameOrEmail { get; set; }

        public string Password { get; set; }
    }

    public class LoginResponseDTO
    {
        public string status { get; set; }
        public string message { get; set; }
        public string accessToken { get; set; }
    }

    public class GetUserResponseDTO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public bool IsLock { get; set; }
        public int FailCount { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }
    }

    public class UnlockUserRequestDTO
    {
        public string UserID { get; set; }
    }

    public class UpdateUserRequestDTO
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
    }

    public class DeleteUserRequestDTO
    {
        public string UserID { get; set; }
    }



    // public class ManageStudentRequestDTO
    // {
    //     public string Operation { get; set; } // "GetStudentDetails" or "BanStudent"
    //     public int CourseID { get; set; }

    //     public int? StudentID { get; set; } // Required for "BanStudent"
    // }
     

    // public class ManageStudentResponseDTO
    // {
    //     public string Status { get; set; }
    //     public string Message { get; set; }
    //     public List<GetStudentsInCourseResponseDTO> Students { get; set; }
    // }

  
}
