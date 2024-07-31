namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("exercise_assignments")]
public class ExerciseAssignment
{
    public int ExerciseAssignmentID { get; set; }

    public int CourseID { get; set; }
    public int ExerciseID { get; set; }

    public int StudentID { get; set; }

    public DateTime AssignedAt { get; set; }
}