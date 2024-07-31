using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public interface IRolePermissionRepository : IRepositoryBase<RolePermission>
    {
        Task<IEnumerable<RolePermission>> GetAllRolePermissions();

        Task<RolePermission?> GetRolePermission(int RoleID, string permission);
    }
}
