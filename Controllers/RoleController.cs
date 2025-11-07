using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.RoleDtos;
using EMS_Backend.Services.RoleFunctionServices;
using EMS_Backend.Services.RoleServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IFunctionRoleService _functionRoleService;

        public RoleController(IRoleService roleService, IFunctionRoleService functionRoleService)
        {
            _roleService = roleService;
            _functionRoleService = functionRoleService;
        }

        [HttpGet ("GetAllFunctionRole")]
        public async Task<IActionResult> GetAllFunctionRole()
        {
            var response = await _functionRoleService.GetAllRoleFunction();
            return Ok(response);
        }

        [HttpPost("GetAllRole")]
        public async Task<IActionResult> GetAllRole([FromBody] PaginationParams @params)
        {
            var response = await _roleService.GetAllRolesAsync(@params);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/GetRoleById")]
        public async Task<IActionResult> GetRoleByIdAsync(string id)
        {
            var response = await _roleService.GetRoleByIdAsync(id);
            if (response.Success == false)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] RoleRequest request)
        {
            var response = await _roleService.AddRoleAsync(request);
            if (response.Success == false)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleRequest request)
        {
            var response = await _roleService.UpdateRoleAsync(request);
            if (response.Success == false)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        [HttpDelete("{id}/DeleteRole")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var response = await _roleService.DeleteRoleAsync(id);
            if (response.Success == false)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
