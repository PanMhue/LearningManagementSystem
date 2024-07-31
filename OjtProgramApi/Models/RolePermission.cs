namespace OjtProgramApi.Models;

[System.ComponentModel.DataAnnotations.Schema.Table("role_permissions")]
public class RolePermission
{
    public int RolePermissionID { get; set; }
    public int RoleID { get; set; }
    public required string Permission { get; set; }
}
