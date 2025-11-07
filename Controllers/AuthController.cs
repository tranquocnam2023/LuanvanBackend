using EMS_Backend.Dtos.AuthDtos;
using EMS_Backend.Helpers;
using EMS_Backend.Services.AuthServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            var response = await _authService.LoginAsync(request);

            if (response.Success == false && response.ErrorCode == ErrorCodes.INVALID_REQUEST)
            {
                return BadRequest(response);
            }

            if (response.Success == false && response.ErrorCode == ErrorCodes.USER_NOT_FOUND)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response.Success == false && response.ErrorCode == ErrorCodes.USER_ALREADY_EXISTS)
            {
                return Conflict(response);
            }
            if (response.Success == false && response.ErrorCode == ErrorCodes.INVALID_REQUEST)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
