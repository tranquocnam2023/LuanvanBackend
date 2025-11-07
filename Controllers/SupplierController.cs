using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.SupplierDtos;
using EMS_Backend.Services.SupplierServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpPost("GetAllSuppliers")]
        public async Task<IActionResult> GetAllSuppliers([FromBody] PaginationParams @params)
        {
            var response = await _supplierService.GetAllSupplierAsync(@params);

            if(!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/GetSupplierById")]
        public async Task<IActionResult> GetSupplierById(string id)
        {
            var response = await _supplierService.GetSupplierByIdAsync(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("AddSupplier")]
        public async Task<IActionResult> AddSupplier([FromBody] SupplierRequest request)
        {
            var response = await _supplierService.AddSupplierAsync(request);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }    
            return Ok(response);
        }

        [HttpPut("UpdateSupplier")]
        public async Task<IActionResult> UpdateSupplier([FromBody] SupplierRequest request)
        {
            var response = await _supplierService.UpdateSupplierAsync(request);
            if(!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteSupplier")]
        public async Task<IActionResult> DeleteSupplier([FromQuery] string id)
        {
            var response = await _supplierService.DeleteSupplierAsync(id);
            if(!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }    
            return Ok(response);
        }
    }
}
