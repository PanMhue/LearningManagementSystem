namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("courses")]
public class Course
{
    public int CourseID { get; set; }

    public string CourseName { get; set; }
    public string Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
