namespace OjtProgramApi.DTO;

public class CreateExerciseDTO
{
    public int CourseID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
}

public class AssignExerciseDTO
{
    public int CourseID { get; set; }   
    public int ExerciseID { get; set; }
    public string  UserID { get; set; }
}

public class MarkExerciseDTO
{
    public int CourseID { get; set; }
    public int ExerciseID { get; set; }
    public string StudentID { get; set; }
    public int Grade { get; set; }
}

public class UserExerciseDTO
{
    public int ExerciseID { get; set; }
    public int CourseID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Grade { get; set; } // Letter grade
}