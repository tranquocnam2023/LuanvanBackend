using EMS_Backend.Dtos;
using EMS_Backend.Dtos.CategoryDtos;
using EMS_Backend.Dtos.PaginationDtos;

namespace EMS_Backend.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<ServiceResponse<PaginationResponse<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams @params);
        Task<ServiceResponse<CategoryResponse>> GetCategoryByIdAsync(int id);
        Task<ServiceResponse<CategoryResponse>> AddCategoryAsync(CategoryRequest request);
        Task<ServiceResponse<CategoryResponse>> DeleteCategoryAsync(int id);
        Task<ServiceResponse<CategoryResponse>> UpdateCategoryAsync(CategoryRequest request);
        Task<ServiceResponse<PaginationResponse<CategoryResponse>>> RecursionAllCategories(PaginationParams @params);
    }
}
