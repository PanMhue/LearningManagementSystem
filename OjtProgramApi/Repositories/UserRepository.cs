using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;

namespace OjtProgramApi.Repositories;

public class UserRepository : RepositoryBase<User>, IUserRepository
{
    public UserRepository(AppDB context)
        : base(context) { }

    public async Task<List<GetUserResponseDTO>> GetAllUsers(string ecbKey)
    {
        var userList = await (
            from user in _context.Users
            join role in _context.Roles on user.RoleID equals role.RoleID
            select new GetUserResponseDTO
            {
                UserID = Encryption.AES_Encrypt_ECB_128(user.userID.ToString(), ecbKey),
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.email,
                IsLock = user.is_lock,
                FailCount = user.login_fail_count,
                RoleID = user.RoleID,
                RoleName = role.RoleName
            }
        ).ToListAsync();

        return userList;
    }

    public async Task<User> GetUserByIdentifierAsync(string identifier)
    {
        string query = @"SELECT * FROM Users WHERE UserName = @Identifier OR Email = @Identifier";
        return (await GetAll<User>(query, new { Identifier = identifier })).FirstOrDefault();
    }

     public async Task<User> GetOldUserByUserNameOrEmailAsync(string userName, string email)
    {
        string query = @"SELECT * FROM Users WHERE UserName = @UserName OR Email = @Email";
        return (await GetAll<User>(query, new { UserName = userName, Email = email })).FirstOrDefault();
    }
}
