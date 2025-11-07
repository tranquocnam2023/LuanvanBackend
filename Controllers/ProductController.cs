using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.ProductDtos;
using EMS_Backend.Services.ProductServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("AddNewProduct")]
        public async Task<IActionResult> AddNewProduct([FromForm] AddProductRequest newProduct)
        {
            var response = await _productService.AddNewProduct(newProduct);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpGet("{id}/GetProductById")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var response = await _productService.GetProductById(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        [HttpPost ("GetAllProduct")]
        public async Task<IActionResult> GetAllProduct([FromBody] PaginationParams @params)
        {
            var response = await _productService.GetAllProduct(@params);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpDelete("{id}/DeleteProduct")]
        public async Task<IActionResult> DeleteProductAsync(string id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
