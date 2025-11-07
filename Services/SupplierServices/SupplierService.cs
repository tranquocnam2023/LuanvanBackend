using Azure;
using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.SupplierDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.GenerateCode;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EMS_Backend.Services.SupplierServices
{
    public class SupplierService : ISupplierService
    {
        private readonly AppDbContext _context;
        private readonly IGenerateCode _generateCode;
        public SupplierService(AppDbContext context, IGenerateCode generateCode)
        {
            _context = context;
            _generateCode = generateCode;
        }

        public async Task<ServiceResponse<SupplierResponse>> AddSupplierAsync(SupplierRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.SupplierName))
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Supplier Request"
                };
            }

            try
            {
                // sinh ma tu dong tu DB proc
                string newSupplierId = await _generateCode.GenerateCodeAsync("SUP", "Suppliers", "Id");

                var newSupplier = new Supplier
                {
                    Id = newSupplierId,
                    SupplierName = request.SupplierName,
                    SupplierAddress = request.SupplierAddress,
                    SupplierPhone = request.SupplierPhone,
                };

                await _context.Suppliers.AddAsync(newSupplier);
                await _context.SaveChangesAsync();

                var supplierResponse = await GetSupplierByIdAsync(newSupplier.Id);

                if (!supplierResponse.Success)
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.SUPPLIER_ADD_CONFLICT,
                        Message = "Error retrieving added supplier."
                    };
                }

                return new ServiceResponse<SupplierResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status201Created,
                    Message = "Supplier added successfully.",
                    Data = supplierResponse.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }


        public async Task<ServiceResponse<SupplierResponse>> DeleteSupplierAsync(string id)
        {
            if(string.IsNullOrWhiteSpace(id))
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Supplier Request"
                };
            }

            try
            {
                var existSuppliers = await _context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);

                if (existSuppliers == null)
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.SUPPLIER_NOT_FOUND,
                        Message = "Supplier not found"
                    };
                }

                _context.Remove(existSuppliers);
                await _context.SaveChangesAsync();

                var response = await GetSupplierByIdAsync(id);

                if(!response.Success)
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = true,
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Supplier deleted successfully"
                    };
                }
                else
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.SUPPLIER_DELETE_CONFLICT,
                        Message = "Error retrieving deleted supplier."
                    };
                }    
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<SupplierResponse>>> GetAllSupplierAsync(PaginationParams @params)
        {
            try
            {
                var query = _context.Suppliers.AsQueryable().AsNoTracking();

                Expression<Func<Supplier, SupplierResponse>> selectors = supplier => new SupplierResponse
                {
                    Id = supplier.Id,
                    SupplierName = supplier.SupplierName,
                    SupplierAddress = supplier.SupplierAddress ?? string.Empty,
                    SupplierPhone = supplier.SupplierPhone ?? string.Empty,
                };

                var result = await PaginationExtension.ToPaginationAsync(query, selectors, @params.PageIndex, @params.PageSize);

                return new ServiceResponse<PaginationResponse<SupplierResponse>>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get all suppliers successfully",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<SupplierResponse>>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<SupplierResponse>> GetSupplierByIdAsync(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status400BadRequest,
                        ErrorCode = ErrorCodes.INVALID_REQUEST,
                        Message = "Invalid request supplier"
                    };
                }

                var result = await _context.Suppliers.AsNoTracking().FirstOrDefaultAsync(su => su.Id == id);

                if (result == null)
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.SUPPLIER_NOT_FOUND,
                        Message = "Supplier not found"
                    };
                }

                var supplier = new SupplierResponse
                {
                    Id = result.Id,
                    SupplierName = result.SupplierName,
                    SupplierAddress = result.SupplierAddress ?? string.Empty,
                    SupplierPhone = result.SupplierPhone ?? string.Empty,
                };

                return new ServiceResponse<SupplierResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Get Supplier By Id Successfully",
                    Data = supplier
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status500InternalServerError,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResponse<SupplierResponse>> UpdateSupplierAsync(SupplierRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Id) || string.IsNullOrWhiteSpace(request.SupplierName))
            {
                return new ServiceResponse<SupplierResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid supplier request"
                };
            }

            try
            {
                var existsSupplier = await _context.Suppliers.FirstOrDefaultAsync(su => su.Id == request.Id);

                if (existsSupplier == null)
                {
                    return new ServiceResponse<SupplierResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.SUPPLIER_NOT_FOUND,
                        Message = "Supplier not found"
                    };
                }

                existsSupplier.SupplierName = request.SupplierName;
                existsSupplier.SupplierAddress = request.SupplierAddress;
                existsSupplier.SupplierPhone = request.SupplierPhone;

                _context.Update(existsSupplier);
                await _context.SaveChangesAsync();

                var response = await GetSupplierByIdAsync(existsSupplier.Id);

                return new ServiceResponse<SupplierResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Update supplier successfully",
                    Data = response.Data
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<SupplierResponse>
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
