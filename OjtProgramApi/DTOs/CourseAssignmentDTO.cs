namespace OjtProgramApi.DTO
{
    public class CourseAssignmentDTO
    {
        public int CourseID { get; set; }
        public int UserID { get; set; }
        public string Role { get; set; } // e.g., 'Instructor' or 'Student'
    }
}
