using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<List<GetUserResponseDTO>> GetAllUsers(string ecbKey);

        Task<User> GetUserByIdentifierAsync(string identifier);

        Task<User> GetOldUserByUserNameOrEmailAsync(string userName, string email);
    }
}
