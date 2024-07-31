using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public class RolePermissionRepository
        : RepositoryBase<RolePermission>,
            IRolePermissionRepository
    {
        public RolePermissionRepository(AppDB context)
            : base(context) { }

        public Task<IEnumerable<RolePermission>> GetAllRolePermissions()
        {
            throw new NotImplementedException();
        }

        public async Task<RolePermission?> GetRolePermission(int roleID, string permission)
        {
            string query =
                "SELECT * FROM role_permissions WHERE RoleID = @RoleID AND Permission = @Permission";
            return (
                await GetAll<RolePermission>(
                    query,
                    new { RoleID = roleID, Permission = permission }
                )
            ).FirstOrDefault();
        }
    }
}
