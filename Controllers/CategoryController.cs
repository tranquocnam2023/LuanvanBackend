using EMS_Backend.Dtos.CategoryDtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Services.CategoryServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace EMS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories([FromBody] PaginationParams @params)
        {
            /*var response = await _categoryService.GetAllCategoriesAsync(@params);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);*/

            var response = await _categoryService.RecursionAllCategories(@params);
            if (response.Success == false)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        [HttpGet("GetCategoryById")]
        public async Task<IActionResult> GetCategoryById([FromQuery] int id)
        {
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory(CategoryRequest request)
        {
            var response = await _categoryService.AddCategoryAsync(request);
            if(!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(CategoryRequest request)
        {
            var response = await _categoryService.UpdateCategoryAsync(request);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory([FromQuery] int id)
        {
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (!response.Success)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
    }
}
