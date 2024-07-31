using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OjtProgramApi.Controllers;
using OjtProgramApi.DTO;
using OjtProgramApi.Models;
using OjtProgramApi.Repositories;
using OjtProgramApi.Util;
using Serilog;

[ApiController]
public class UserController : BaseController<UserController>
{
    private readonly IRepositoryWrapper _repositoryWrapper;
    private readonly IConfiguration _configuration;

    public UserController(IRepositoryWrapper repositoryWrapper, IConfiguration configuration)
    {
        _repositoryWrapper = repositoryWrapper;
        _configuration = configuration;
    }

    [HttpGet("GetAllUsers", Name = "GetAllUsers")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<List<GetUserResponseDTO>>> GetAllUsers()
    {
        Logger?.LogInformation("GetAllUser method called");

        string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

        try
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userList = await _repositoryWrapper.User.GetAllUsers(ecbKey);
            if (userList == null || !userList.Any())
            {
                return NotFound();
            }
            await _repositoryWrapper.EventLog.UpdateEventLog(
                "UserController",
                "GetAllUsers",
                "Admin viewed all users"
            );
            return Ok(userList);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in GetAllUsers method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "GetAllUsers",
                "Failed to get all users",
                ex.Message
            );
        }
        return StatusCode(500, "Internal server error");
    }

    [HttpPost("CreateUser", Name = "CreateUser")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> CreateUser(UserRequestDTO user)
    {
        Logger?.LogInformation("CreateUser method called");

        CreateUserResponseDTO createUserResponse = new CreateUserResponseDTO();
        
        try
        {
            var salt = GlobalFunction.GenerateSalt();

            var oldUser = await _repositoryWrapper.User.GetOldUserByUserNameOrEmailAsync(user.UserName, user.Email);

            if (oldUser != null)
            {
                return Ok(
                    new CreateUserResponseDTO { status = "fail", message = "User already exists" }
                );
            }
            else
            {
                var newUser = new User
                {
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    email = user.Email,
                    password = GlobalFunction.ComputeHash(salt, user.Password),
                    salt = salt,
                    login_fail_count = 0,
                    is_lock = false,
                    RoleID = user.RoleID,
                    CreatedAt = DateTime.Now
                };

                _repositoryWrapper.User.Create(newUser);
                await _repositoryWrapper.User.Save();
                await _repositoryWrapper.EventLog.InsertEventLog(
                    "UserController",
                    "CreateUser",
                    $"User created: {user.UserName}"
                );
                return Ok(
                    new CreateUserResponseDTO
                    {
                        status = "success",
                        message = "User created successfully"
                    }
                );
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in Createuser method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "CreateUsers",
                "Failed to create user",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("Login", Name = "Login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO user)
    {
        Logger?.LogInformation("Login method called");

        LoginResponseDTO loginResponseDTO = new LoginResponseDTO();
        try
        {
            var oldUser = await _repositoryWrapper.User.GetUserByIdentifierAsync(
                user.UserNameOrEmail
            );

            if (oldUser == null)
            {
                loginResponseDTO = new LoginResponseDTO
                {
                    status = "fail",
                    message = "Username or Password is incorrect"
                };
            }
            else
            {
                if (oldUser.is_lock)
                {
                    loginResponseDTO = new LoginResponseDTO
                    {
                        status = "fail",
                        message = "Account is locked due to multiple failed login attempts"
                    };
                    return new OkObjectResult(loginResponseDTO);
                }
                var hash = GlobalFunction.ComputeHash(oldUser.salt, user.Password);
                if (hash == oldUser.password)
                {
                    oldUser.login_fail_count = 0;
                    _repositoryWrapper.User.Update(oldUser);
                    await _repositoryWrapper.User.Save();
                    await _repositoryWrapper.EventLog.UpdateEventLog(
                        "UserController",
                        "Login",
                        $"User: {oldUser.UserName} logged in"
                    );

                    Claim[] claims = GlobalFunction.CreateClaim(
                        oldUser.userID,
                        oldUser.RoleID,
                        new DateTimeOffset(DateTime.UtcNow)
                            .ToUniversalTime()
                            .ToUnixTimeSeconds()
                            .ToString()
                    );
                    var token = GlobalFunction.CreateJWTToken(claims);
                    loginResponseDTO = new LoginResponseDTO
                    {
                        status = "success",
                        message = "Login success",
                        accessToken = token
                    };
                }
                else
                {
                    oldUser.login_fail_count += 1;
                    if (
                        oldUser.login_fail_count
                        >= Convert.ToInt32(_configuration["LoginSettings:MaxLoginFailCount"])
                    )
                    {
                        oldUser.is_lock = true;
                    }
                    _repositoryWrapper.User.Update(oldUser);
                    await _repositoryWrapper.User.Save();
                    await _repositoryWrapper.EventLog.UpdateEventLog(
                        "UserController",
                        "Login",
                        $"User: {oldUser.UserName} failed to login"
                    );

                    loginResponseDTO = new LoginResponseDTO
                    {
                        status = "fail",
                        message = "Username or Password is incorrect"
                    };
                }
            }
            return new OkObjectResult(loginResponseDTO);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in UserLogin method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "Login",
                "Failed to login",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("UpdateUser", Name = "UpdateUser")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> UpdateUser(
        UpdateUserRequestDTO updateRequest
    )
    {
        Logger?.LogInformation("UpdateUser method called");

        try
        {
            var encryptedKey = updateRequest.UserID;
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

            int userID = int.Parse(Encryption.AES_Decrypt_ECB_128(encryptedKey, ecbKey));
            var oldUser = await _repositoryWrapper.User.FindByID(userID);
            if (oldUser == null)
            {
                return Ok(
                    new CreateUserResponseDTO
                    {
                        status = "fail",
                        message = "Invalid encrypted author ID"
                    }
                );
            }
            else
            {
                oldUser.UserName = updateRequest.UserName;
                _repositoryWrapper.User.Update(oldUser);
                await _repositoryWrapper.User.Save();
                await _repositoryWrapper.EventLog.UpdateEventLog(
                    "UserController",
                    "UpdateUser",
                    $"User: {oldUser.UserName} updated"
                );
                return Ok(
                    new CreateUserResponseDTO
                    {
                        status = "success",
                        message = "User updated successfully"
                    }
                );
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in UpdateUser method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "UpdateUser",
                "Failed to update user",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("UnlockUser", Name = "UnlockUser")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> UnlockUser(UnlockUserRequestDTO id)
    {
        Logger?.LogInformation("UnlockUser method called");

        CreateUserResponseDTO createUserResponse = new CreateUserResponseDTO();
        try
        {
            var encryptedKey = id.UserID;
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

            int userID = int.Parse(Encryption.AES_Decrypt_ECB_128(encryptedKey, ecbKey));
            var oldUser = await _repositoryWrapper.User.FindByID(userID);
            if (oldUser == null)
            {
                return Ok(
                    new CreateUserResponseDTO { status = "fail", message = "User not found" }
                );
            }
            else
            {
                oldUser.is_lock = false;
                oldUser.login_fail_count = 0;
                _repositoryWrapper.User.Update(oldUser);
                await _repositoryWrapper.User.Save();
                await _repositoryWrapper.EventLog.UpdateEventLog(
                    "UserController",
                    "UnlockUser",
                    $"User: {oldUser.UserName} unlocked"
                );
                return Ok(
                    new CreateUserResponseDTO
                    {
                        status = "success",
                        message = "User unlocked successfully"
                    }
                );
            }
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in UnlockUser method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "UnlockUser",
                "Failed to unlock user",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("DeleteUser", Name = "DeleteUser")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<ActionResult<CreateUserResponseDTO>> DeleteUser(DeleteUserRequestDTO id)
    {
        Logger?.LogInformation("DeleteUser method called");

        try
        {
            var encryptedKey = id.UserID;
            string ecbKey = _configuration.GetValue<string>("Encyption:ECBSecretKey") ?? "";

            int userID = int.Parse(Encryption.AES_Decrypt_ECB_128(encryptedKey, ecbKey));
            var oldUser = await _repositoryWrapper.User.FindByID(userID);
            if (oldUser == null)
            {
                return NotFound(
                    new CreateUserResponseDTO { status = "fail", message = "User not found" }
                );
            }

            _repositoryWrapper.User.Delete(oldUser);
            await _repositoryWrapper.User.Save();
            await _repositoryWrapper.EventLog.DeleteEventLog(
                "UserController",
                "DeleteUser",
                $"User: {oldUser.UserName} deleted"
            );

            return Ok(
                new CreateUserResponseDTO
                {
                    status = "success",
                    message = "User deleted successfully"
                }
            );
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error in DeleteUser method");
            await _repositoryWrapper.EventLog.ErrorEventLog(
                "UserController",
                "DeleteUser",
                "Failed to delete user",
                ex.Message
            );
            return StatusCode(500, "Internal server error");
        }
    }
} //GetStudentDetails (instructor)
//BanStudent(instructor)
