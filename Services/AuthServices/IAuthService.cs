using EMS_Backend.Dtos;
using EMS_Backend.Dtos.AuthDtos;

namespace EMS_Backend.Services.AuthServices
{
    public interface IAuthService
    {
        Task<ServiceResponse<AuthResponse>> LoginAsync(AuthRequest loginRequest);
        Task<ServiceResponse<AuthResponse>> RegisterAsync(RegisterRequest registerRequest);
    }
}
