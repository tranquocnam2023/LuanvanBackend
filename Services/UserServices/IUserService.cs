using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.UserDtos;

namespace EMS_Backend.Services.UserServices
{
    public interface IUserService
    {
        Task<ServiceResponse<PaginationResponse<UserResponse>>> GetAllUsers(PaginationParams @params);
        Task<ServiceResponse<UserResponse>> GetUserById(string username);
        Task<ServiceResponse<UserResponse>> AddUserAsync(AddNewUserRequest request);
        Task<ServiceResponse<UserResponse>> DeleteUserAsync(string username);
        Task<ServiceResponse<UserResponse>> UpdateUserAsync(UserRequest request);
    }
}
