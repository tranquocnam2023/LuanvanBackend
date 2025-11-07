using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.UserDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.GenerateCode;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IGenerateCode _generateCode;
        public UserService(AppDbContext context, IGenerateCode generateCode)
        {
            _context = context;
            _generateCode = generateCode;
        }

        public async Task<ServiceResponse<UserResponse>> AddUserAsync(AddNewUserRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.FullName) || string.IsNullOrWhiteSpace(request.RoleId))
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid user request."
                };
            }

            try
            {
                 string userId = await _generateCode.GenerateCodeAsync(request.RoleId, "Users", "UserId"); 

                var newUser = new User
                {
                    UserId = userId,
                    Password = userId,
                    FullName = request.FullName,
                    RoleId = request.RoleId
                };
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();

                var response = await GetUserById(userId);

                if (!response.Success)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.USER_ADD_CONFLICT,
                        Message = "Error retrieving added user."
                    };
                }

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "User added successfully.",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<UserResponse>> DeleteUserAsync(string username)
        {
            if (username == null || string.IsNullOrWhiteSpace(username))
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid username."
                };
            }

            var userExisting = await _context.Users.FirstOrDefaultAsync(u => u.UserId == username);
            if (userExisting == null)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorCode = ErrorCodes.USER_NOT_FOUND,
                    Message = "User not found."
                };
            }

            try
            {
                _context.Users.Remove(userExisting);
                await _context.SaveChangesAsync();
                var response = await GetUserById(username);
                if (!response.Success)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "User deleted successfully."
                    };
                }
                else
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.USER_DELETE_CONFLICT,
                        Message = "Error retrieving deleted user."
                    };
                }    
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<UserResponse>>> GetAllUsers(PaginationParams @params)
        {
            try
            {
                var query = _context.Users.AsQueryable().AsNoTracking();

                Expression<Func<User, UserResponse>> selectors = user => new UserResponse
                {
                    Username = user.UserId,
                    FullName = user.FullName,
                    RoleId = user.RoleId,
                    RoleName = user.Role!.RoleName ?? string.Empty
                };
                
                var results = await PaginationExtension.ToPaginationAsync<User, UserResponse>(query, selectors, @params.PageIndex, @params.PageSize);

                return new ServiceResponse<PaginationResponse<UserResponse>>
                {                    
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get All Users Successfully",
                    Data = results
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<PaginationResponse<UserResponse>>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<UserResponse>> GetUserById(string username)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid username."
                    };
                }

                //var request = await _context.Users.AsNoTracking().Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == username);
                var userResponse = await _context.Users.AsNoTracking()
                              .Where(u => u.UserId == username)
                              .Select(u => new UserResponse
                              {
                                  Username = u.UserId,
                                  FullName = u.FullName,
                                  RoleId = u.RoleId,
                                  RoleName = u.Role == null ? string.Empty : u.Role.RoleName.ToString(),
                              }).FirstOrDefaultAsync();

                if (userResponse == null)
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.USER_NOT_FOUND,
                        Message = "User not found."
                    };
                }
            
                //var userResponse = new UserResponse
                //{
                //    Username = request.UserName,
                //    FullName = request.FullName,
                //    RoleId = request.RoleId,
                //    RoleName = request.Role!.RoleName ?? string.Empty
                //};

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get User Successfully",
                    Data = userResponse
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<UserResponse>> UpdateUserAsync(UserRequest request)
        {
            if(request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.RoleId))
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid user request."
                };
            }
            
            var userExisting = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.Username);

            if (userExisting == null)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorCode = ErrorCodes.USER_NOT_FOUND,
                    Message = "User not found."
                };
            }

            try
            {
                
                userExisting.Password = request.Password == null? userExisting.Password : request.Password;
                userExisting.FullName = request.FullName;

                if (request.RoleId == "Administrator" || request.RoleId == "Manager")
                {
                    userExisting.RoleId = request.RoleId;
                }

                _context.Users.Update(userExisting);
                await _context.SaveChangesAsync();

                var response = await GetUserById(request.Username);

                if(!response.Success || !userExisting.FullName.Equals(response.Data.FullName) || !userExisting.RoleId.Equals(response.Data.RoleId))
                {
                    return new ServiceResponse<UserResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.USER_UPDATE_CONFLICT,
                        Message = "Error retrieving updated user."
                    };
                } // :)) ??

                return new ServiceResponse<UserResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "User updated successfully.",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<UserResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }
    }
}
