using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.CategoryDtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.GenerateCode;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;
        private readonly IGenerateCode _generateCode;
        public CategoryService(AppDbContext context, IGenerateCode generateCode)
        {
            _context = context;
            _generateCode = generateCode;
        }

        public async Task<ServiceResponse<CategoryResponse>> AddCategoryAsync(CategoryRequest request)
        {

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid Category Request",
                    };
                }

                /// Kiểm tra category đã tồn tại chưa theo tên 
                /// (có thể có nhiều category cùng tên nhưng trong trường hợp này ta giả sử không được trùng tên)
                /*
                var exists = await _context.Categories.AnyAsync(c => c.Name == request.CategoryName);

                if(exists)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.CATEGORY_ADD_CONFLICT,
                        Message = "Category Already Exists",
                    };
                }
                */

                var newCategory = new Category
                {
                    Name = request.CategoryName,
                    ParentCategoryId = request.ParentCategoryId > 0 ? request.ParentCategoryId : null
                };

                await _context.Categories.AddAsync(newCategory);
                await _context.SaveChangesAsync();

                var categoryResponse = await GetCategoryByIdAsync(newCategory.Id);
                if(categoryResponse == null)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.CATEGORY_ADD_CONFLICT,
                        Message = "Error retrieving added category."
                    };
                }

                return new ServiceResponse<CategoryResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Category added successfully",
                    Data = categoryResponse.Data
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<CategoryResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<CategoryResponse>> DeleteCategoryAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid Category Id",
                    };
                }

                var existCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

                if (existCategory == null)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.CATEGORY_NOT_FOUND,
                        Message = "Category Not Found"
                    };
                }

                _context.Categories.Remove(existCategory);
                await _context.SaveChangesAsync();

                var response = await GetCategoryByIdAsync(id);
                if(!response.Success)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Category deleted successfully."
                    };
                }
                else
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.CATEGORY_DELETE_CONFLICT,
                        Message = "Error retrieving deleted category."
                    };
                }    
            }
            catch (Exception ex)
            {

                return new ServiceResponse<CategoryResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams @params)
        {
            try
            {
                var query = _context.Categories.AsQueryable().AsNoTracking();

                Expression<Func<Category, CategoryResponse>> selectors = category => new CategoryResponse
                {
                    Id = category.Id,
                    CategoryName = category.Name,
                    ParentCategoryId = category.ParentCategoryId ?? 0
                };

                var results = await PaginationExtension.ToPaginationAsync<Category, CategoryResponse>(query, selectors, @params.PageIndex, @params.PageSize);

                return new ServiceResponse<PaginationResponse<CategoryResponse>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get All Categories Successfully",
                    Data = results
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<PaginationResponse<CategoryResponse>>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<CategoryResponse>> GetCategoryByIdAsync(int id)
        {
            try
            {
                if(id <= 0 || string.IsNullOrWhiteSpace(id.ToString())) // ??
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid Category Id",
                    };
                }

                var allCategory = await _context.Categories.AsNoTracking().ToListAsync();

                var root = allCategory.FirstOrDefault(cate => cate.Id == id);

                if(root == null)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.CATEGORY_NOT_FOUND,
                        Message = "Category Not Found",
                    };
                }

                var response = new CategoryResponse
                {
                    Id = root.Id,
                    CategoryName = root.Name,
                    childCates = RecursionCategory(allCategory,root.Id),
                };

                return new ServiceResponse<CategoryResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get Category By Id Successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {

                return new ServiceResponse<CategoryResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<CategoryResponse>>> RecursionAllCategories(PaginationParams @params)
        {
            //throw new NotImplementedException();
            try
            {
                var allCategories = await _context.Categories.AsNoTracking().ToListAsync();

                var parentCategory = allCategories.Where(x => x.ParentCategoryId == null || x.ParentCategoryId == 0)
                                     .Skip((@params.PageIndex - 1) * @params.PageSize)
                                     .Take(@params.PageSize)
                                     .ToList();

                var count = allCategories.Count(x => x.ParentCategoryId == null);

                // recursions
                var categoryResponse = parentCategory.Select(x => new CategoryResponse
                {
                    Id = x.Id,
                    CategoryName = x.Name,
                    ParentCategoryId = x.ParentCategoryId ?? 0,
                    childCates = RecursionCategory(allCategories, x.Id)
                }).ToList();

                var pagination = new PaginationResponse<CategoryResponse>(categoryResponse, count, @params.PageIndex, @params.PageSize);

                return new ServiceResponse<PaginationResponse<CategoryResponse>>
                {
                    Success = true,
                    Data = pagination,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get Categories Successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<CategoryResponse>>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        private List<CategoryResponse> RecursionCategory(List<Category> items, int parentId)
        {
            var results = new List<CategoryResponse>();

            foreach (var item in items)
            {
                if (item.ParentCategoryId == parentId)
                {
                    results.Add(new CategoryResponse
                    {
                        Id = item.Id,
                        CategoryName = item.Name,
                        ParentCategoryId = item.ParentCategoryId ?? 0,
                        childCates = RecursionCategory(items, item.Id)
                    });
                }
            }

            return results;
        }

        public async Task<ServiceResponse<CategoryResponse>> UpdateCategoryAsync(CategoryRequest request)
        {
            try
            {
                if(request == null || string.IsNullOrWhiteSpace(request.CategoryName))
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid Category Request"
                    };
                }

                var exists = await _context.Categories.FirstOrDefaultAsync(c => c.Id == request.Id);

                if (exists == null)
                {
                    return new ServiceResponse<CategoryResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.CATEGORY_NOT_FOUND,
                        Message = "Category Not Found"
                    };
                }

                exists.Name = request.CategoryName;
                exists.ParentCategoryId = request.ParentCategoryId;

                _context.Categories.Update(exists);
                await _context.SaveChangesAsync();

                var response = await GetCategoryByIdAsync(exists.Id);



                return new ServiceResponse<CategoryResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Update Category Successfully",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<CategoryResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }
    }
}
