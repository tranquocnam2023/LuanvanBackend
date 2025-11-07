using EMS_Backend.Dtos.FunctionDtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Services.FunctionServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FunctionController : ControllerBase
    {
        private readonly IFunctionService _functionService;
        public FunctionController(IFunctionService functionService)
        {
            _functionService = functionService;
        }

        [HttpPost("AddNewFunction")]
        public async Task<IActionResult> AddNewFunction([FromBody] FunctionRequest functionRequest)
        {
            var response = await _functionService.AddNewFunction(functionRequest);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/GetFunctionById")]
        public async Task<IActionResult> GetFunctionById(string id)
        {
            var response = await _functionService.GetFunctionById(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("GetAllFunctions")]
        public async Task<IActionResult> GetAllFunctions([FromBody] PaginationParams @params)
        {
            var response = await _functionService.GetAllFunctions(@params);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateFunction")]
        public async Task<IActionResult> UpdateFunction([FromBody] FunctionRequest functionRequest)
        {
            var response = await _functionService.UpdateFunction(functionRequest);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        [HttpDelete("{id}/DeleteFunction")]
        public async Task<IActionResult> DeleteFunction(string id)
        {
            var response = await _functionService.DeleteFunction(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
