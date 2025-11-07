using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.ProductDtos;

namespace EMS_Backend.Services.ProductServices
{
    public interface IProductService
    {
        Task<ServiceResponse<ProductResponse>> AddNewProduct(AddProductRequest newProduct);
        Task<ServiceResponse<ProductResponse>> GetProductById (string productId);
        Task<ServiceResponse<PaginationResponse<ProductResponse>>> GetAllProduct(PaginationParams @params);
        Task<ServiceResponse<ProductResponse>> DeleteProductAsync(string productId);
    }
}
