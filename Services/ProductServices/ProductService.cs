using Azure.Core;
using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.ProductDtos;
using EMS_Backend.Dtos.UserDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.GenerateCode;
using EMS_Backend.Services.ImageProductServices;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly IImageProductService _imageProductService;
        private readonly AppDbContext _context;
        private readonly IGenerateCode _generateCode;
        public ProductService(IImageProductService image, AppDbContext context, IGenerateCode generateCode)
        {
            _imageProductService = image;
            _context = context;
            _generateCode = generateCode;
        }

        private string SetFileName(string productCode, IFormFile file)
        {
            var folder = productCode;
            var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var safeFileName = Path.GetFileName(file.FileName);

            var fileName = $"{folder}/{timeStamp}_{safeFileName}";
            return fileName;
        }

        public async Task<ServiceResponse<ProductResponse>> AddNewProduct(AddProductRequest newProduct)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(newProduct.ProductName)  || newProduct.BasePrice == 0)
            {
                return new ServiceResponse<ProductResponse>
                {
                    Data = null!,
                    Success = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            try
            {
                string prefix = $"{newProduct.SupplierId}-PRO";
                string productId = await _generateCode.GenerateCodeAsync(prefix, "Product", "ProductId"); 
                string filename = SetFileName(productId, newProduct.fileImageThumbnail);
                var imageUrl = await _imageProductService.UploadImageAsync(newProduct.fileImageThumbnail, filename);

                var newProductEntity = new Product
                {
                    ProductId = productId,
                    ProductName = newProduct.ProductName,
                    BasePrice = newProduct.BasePrice,
                    CategoryId = newProduct.CategoryId,
                    SupplierId = newProduct.SupplierId,
                    ThumbnailUrl = imageUrl,
                    IsVariant = newProduct.IsVariant
                };

                await _context.Product.AddAsync(newProductEntity);
                await _context.SaveChangesAsync();

                var resposne = await GetProductById(productId);
                if (resposne.Success == false)
                {
                    return new ServiceResponse<ProductResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.USER_ADD_CONFLICT,
                        Message = "Error retrieving added user."
                    };
                }

                return new ServiceResponse<ProductResponse>
                {
                    Data = resposne.Data,
                    Success = true,
                    Message = "Product added successfully.",
                    StatusCode = StatusCodes.Status201Created
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ProductResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<ProductResponse>> GetProductById(string productId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new ServiceResponse<ProductResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Product ID is required."
                };
            }

            try
            {
                var productResponse = await _context.Product.AsNoTracking()
                                       .Where(p => p.ProductId == productId)
                                       .Select(p => new ProductResponse
                                       {
                                           ProductId = p.ProductId,
                                           ProductName = p.ProductName,
                                           BasePrice = p.BasePrice,
                                           CateId = p.CategoryId,
                                           CateName = p.Category != null ? p.Category.Name : string.Empty,
                                           SupplierId = p.SupplierId,
                                           SupplierName = p.Supplier != null ? p.Supplier.SupplierName : string.Empty,
                                           ThumbnailUrl = p.ThumbnailUrl
                                       })
                                        .FirstOrDefaultAsync();
                if (productResponse == null)
                {
                    return new ServiceResponse<ProductResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.RESOURCE_NOT_FOUND,
                        Message = "Product not found."
                    };
                }
                return new ServiceResponse<ProductResponse>
                {
                    Data = productResponse,
                    Success = true,
                    Message = "Product retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<ProductResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<ProductResponse>>> GetAllProduct(PaginationParams @params)
        {
            //throw new NotImplementedException();
            try
            {
                var query = _context.Product.AsQueryable();
                Expression<Func<Product, ProductResponse>> selectors = product => new ProductResponse
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    BasePrice = product.BasePrice,
                    CateId = product.CategoryId,
                    CateName = product.Category != null ? product.Category.Name : string.Empty,
                    SupplierId = product.SupplierId,
                    SupplierName = product.Supplier != null ? product.Supplier.SupplierName : string.Empty,
                    ThumbnailUrl = product.ThumbnailUrl
                };

                var result = await PaginationExtension.ToPaginationAsync<Product, ProductResponse>(query, selectors, @params.PageIndex, @params.PageSize);
                return new ServiceResponse<PaginationResponse<ProductResponse>>
                {
                    Data = result,
                    Success = true,
                    Message = "Products retrieved successfully.",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<ProductResponse>>
                {
                    Data = null!,
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ServiceResponse<ProductResponse>> DeleteProductAsync(string productId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(productId))
            {
                return new ServiceResponse<ProductResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Product ID is required."
                };
            }
            var existProduct = await _context.Product.FindAsync(productId);
            if (existProduct == null)
            {
                return new ServiceResponse<ProductResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorCode = ErrorCodes.PRODUCT_NOT_FOUND,
                    Message = "Product not found."
                };
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existProductImage = await _context.ProductImage.Where(x => x.ProductId == productId).ToListAsync();

                    if (existProductImage.Count > 0)
                    {
                        _context.ProductImage.RemoveRange(existProductImage);
                    }

                    _context.Product.Remove(existProduct);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    try
                    {
                        await _imageProductService.DeleteImageInFolder(productId);
                    }
                    catch (Exception ex)
                    {
                       return new ServiceResponse<ProductResponse>
                        {
                            Success = false,
                            StatusCode = StatusCodes.Status500InternalServerError,
                            ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                            Message = $"Product deleted but failed to delete images from storage: {ex.Message}"
                        };
                    }


                    return new ServiceResponse<ProductResponse>
                    {
                        Data = null!,
                        Success = true,
                        Message = "Product deleted successfully.",
                        StatusCode = StatusCodes.Status200OK
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse<ProductResponse>
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
}
