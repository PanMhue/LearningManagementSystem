using System.ComponentModel.DataAnnotations;

namespace OjtProgramApi.Models
{
    [System.ComponentModel.DataAnnotations.Schema.Table("users")]
    public class User
    {
        [Key]
        public int userID { get; set; }

        [Required]
        [MaxLength(200)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(200)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(200)]
        public string LastName { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        [MaxLength(200)]
        public string password { get; set; }

        [Required]
        [MaxLength(500)]
        public string salt { get; set; }

        [Required]
        public int login_fail_count { get; set; }

        [Required]
        public bool is_lock { get; set; }

        [Required]
        public int RoleID { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
