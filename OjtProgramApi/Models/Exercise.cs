namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("exercises")]
public class Exercise
{
    public int ExerciseID { get; set; }
    public int CourseID { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }
}
