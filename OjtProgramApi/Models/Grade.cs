using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("grades")]
    public class Grade
    {
        [Key]
        public int GradeID { get; set; }
        
        [ForeignKey("Exercise")]
        public int ExerciseID { get; set; }
        
        [ForeignKey("Student")]
        public int StudentID { get; set; }
        
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }
        
        public int GradeValue { get; set; }
        
        public DateTime GradedAt { get; set; }
        
        public virtual Exercise Exercise { get; set; }
        
        public virtual User Student { get; set; }
        
        public virtual User Instructor { get; set; }
    }

