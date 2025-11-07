using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.AuthDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using Microsoft.EntityFrameworkCore;

namespace EMS_Backend.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<AuthResponse>> LoginAsync(AuthRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return new ServiceResponse<AuthResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid login request."
                };
            }

            var request = await _context.Users.FirstOrDefaultAsync(u => u.UserId == loginRequest.Username && u.Password == loginRequest.Password);
            if (request == null)
            {
                return new ServiceResponse<AuthResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorCode = ErrorCodes.USER_NOT_FOUND,
                    Message = "Invalid username or password."
                };
            }

            var userResponse = new AuthResponse
            {
                UserName = request.UserId,
                FullName = request.FullName,
                RoleId = request.RoleId
            };

            return new ServiceResponse<AuthResponse>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Message = "Login successful.",
                Data = userResponse
            };
        }

        public async Task<ServiceResponse<AuthResponse>> RegisterAsync(RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrWhiteSpace(registerRequest.Username) || string.IsNullOrWhiteSpace(registerRequest.Password) || string.IsNullOrWhiteSpace(registerRequest.FullName))
            {
                return new ServiceResponse<AuthResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid registration request.",
                    Data = null!
                };
            }

            if (await _context.Users.AnyAsync(u => u.UserId == registerRequest.Username))
            {
                return new ServiceResponse<AuthResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status409Conflict,
                    ErrorCode = ErrorCodes.USER_ALREADY_EXISTS,
                    Message = "Username already exists.",
                    Data = null!
                };
            }

            var newUser = new User
            {
                UserId = registerRequest.Username,
                Password = registerRequest.Password,
                FullName = registerRequest.FullName,
                RoleId = "Customer" // Default role
            };

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            var userResponse = new AuthResponse
            {
                UserName = newUser.UserId,
                FullName = newUser.FullName,
                RoleId = newUser.RoleId
            };

            return new ServiceResponse<AuthResponse>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = "Registration successful.",
                Data = userResponse
            };
        }
    }
}
