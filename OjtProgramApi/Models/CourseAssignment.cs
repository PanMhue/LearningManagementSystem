namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("course_assignments")]
public class CourseAssignment
{
    public int AssignmentID { get; set; }
    public int CourseID { get; set; }
    public int UserID { get; set; }

}
