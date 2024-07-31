namespace OjtProgramApi.DTO
{
    public class CreateGradeDTO
    {
        public int ExerciseID { get; set; }
        public int StudentID { get; set; }
        public int Grade { get; set; }
    }

    public class UserGradeDTO
    {
        public int GradeID { get; set; }
        public int ExerciseID { get; set; }
        public string ExerciseTitle { get; set; }
       

        public string Grade { get; set; }



    }


}
