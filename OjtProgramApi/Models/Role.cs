namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("roles")]
public class Role
{
    public int RoleID { get; set; }
    public required string RoleName { get; set; }
}
