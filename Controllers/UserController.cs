using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.UserDtos;
using EMS_Backend.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers([FromBody] PaginationParams @params)
        {
            var response = await _userService.GetAllUsers(@params);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserById([FromQuery] string? username)
        {
            var response = await _userService.GetUserById(username ?? string.Empty);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("AddUser")]
        public async Task<IActionResult> AddUser(AddNewUserRequest request)
        {
            var response = await _userService.AddUserAsync(request);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromQuery] string? username)
        {
            var response = await _userService.DeleteUserAsync(username ?? string.Empty);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser(UserRequest request)
        {
            var response = await _userService.UpdateUserAsync(request);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
