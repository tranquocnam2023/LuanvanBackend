using EMS_Backend.Data;
using EMS_Backend.Dtos;
using EMS_Backend.Dtos.FunctionDtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Entities;
using EMS_Backend.Helpers;
using EMS_Backend.Services.GenerateCode;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EMS_Backend.Services.FunctionServices
{
    public class FunctionService : IFunctionService
    {
        private readonly AppDbContext _context;
        private readonly IGenerateCode _generateCode;

        public FunctionService(AppDbContext context, IGenerateCode generateCode)
        {
            _context = context;
            _generateCode = generateCode;
        }

        public async Task<ServiceResponse<FunctionResponse>> AddNewFunction(FunctionRequest functionRequest)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(functionRequest.FunctionName))
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }
            try
            {
                var functionId = await _generateCode.GenerateCodeAsync("FUNC", "FunctionMasters", "FunctionId");
                var newFunction = new FunctionMaster
                {
                    FunctionId = functionId,
                    FunctionName = functionRequest.FunctionName.Trim()
                };
                await _context.FunctionMasters.AddAsync(newFunction);
                await _context.SaveChangesAsync();

                var response = await GetFunctionById(functionId);
                if (response.Success == false)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.FUNCTION_ADD_CONFLICT,
                        Message = "Error retrieving added function."
                    };
                }

                return new ServiceResponse<FunctionResponse>
                {
                    Data = response.Data,
                    Success = true,
                    Message = "Function added successfully.",
                    StatusCode = StatusCodes.Status201Created
                };

            }
            catch (Exception ex)
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<FunctionResponse>> DeleteFunction(string functionId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(functionId))
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    StatusCode = StatusCodes.Status400BadRequest,
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    Message = "Invalid Function Request"
                };
            }

            try
            {
                var existFunctions = await _context.FunctionMasters.FirstOrDefaultAsync(f => f.FunctionId == functionId);
                if (existFunctions == null)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        ErrorCode = ErrorCodes.FUNCTION_NOT_FOUND,
                        Message = "Function not found"
                    };
                }

                _context.FunctionMasters.Remove(existFunctions);
                await _context.SaveChangesAsync();
                var response = await GetFunctionById(functionId);
                if (response.Success == true)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.FUNCTION_DELETE_CONFLICT,
                        Message = "Error deleting function."
                    };
                }

                return new ServiceResponse<FunctionResponse>
                {
                    Success = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "Function deleted successfully"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<PaginationResponse<FunctionResponse>>> GetAllFunctions(PaginationParams @params)
        {
            //throw new NotImplementedException();
            try
            {
                var query = _context.FunctionMasters.AsQueryable().AsNoTracking();
                Expression<Func<FunctionMaster, FunctionResponse>> selectors = function => new FunctionResponse
                {
                    FunctionId = function.FunctionId,
                    FunctionName = function.FunctionName
                };

                var result = await PaginationExtension.ToPaginationAsync(query, selectors, @params.PageIndex, @params.PageSize);
                return new ServiceResponse<PaginationResponse<FunctionResponse>>
                {
                    Data = result,
                    Success = true,
                    Message = "Functions retrieved successfully",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<PaginationResponse<FunctionResponse>>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<FunctionResponse>> GetFunctionById(string functionId)
        {
            //throw new NotImplementedException();
            if (string.IsNullOrWhiteSpace(functionId))
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = "Invalid Function ID",
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            try
            {
                var functionResponse = await _context.FunctionMasters
                    .Where(f => f.FunctionId == functionId)
                    .Select(f => new FunctionResponse
                    {
                        FunctionId = f.FunctionId,
                        FunctionName = f.FunctionName
                    })
                    .FirstOrDefaultAsync();
                if (functionResponse == null)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        Message = "Function not found",
                        ErrorCode = ErrorCodes.FUNCTION_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                return new ServiceResponse<FunctionResponse>
                {
                    Data = functionResponse,
                    Success = true,
                    Message = "Function retrieved successfully",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<FunctionResponse>> UpdateFunction(FunctionRequest functionRequest)
        {
            //throw new NotImplementedException();
            if (functionRequest == null || string.IsNullOrWhiteSpace(functionRequest.FunctionId) || string.IsNullOrWhiteSpace(functionRequest.FunctionName))
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = "Invalid input data",
                    ErrorCode = ErrorCodes.INVALID_REQUEST,
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            try
            {
                var functionExisting = await _context.FunctionMasters
                    .FirstOrDefaultAsync(f => f.FunctionId == functionRequest.FunctionId);
                if (functionExisting == null)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        Message = "Function not found",
                        ErrorCode = ErrorCodes.FUNCTION_NOT_FOUND,
                        StatusCode = StatusCodes.Status404NotFound
                    };
                }

                functionExisting.FunctionName = functionRequest.FunctionName.Trim();
                await _context.SaveChangesAsync();
                var response = await GetFunctionById(functionRequest.FunctionId);
                if (response.Success == false)
                {
                    return new ServiceResponse<FunctionResponse>
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status409Conflict,
                        ErrorCode = ErrorCodes.FUNCTION_UPDATE_CONFLICT,
                        Message = "Error retrieving updated function."
                    };
                }
                return new ServiceResponse<FunctionResponse>
                {
                    Data = response.Data,
                    Success = true,
                    Message = "Function updated successfully.",
                    StatusCode = StatusCodes.Status200OK
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<FunctionResponse>
                {
                    Success = false,
                    Message = ex.Message,
                    ErrorCode = ErrorCodes.INTERNAL_SERVER_ERROR,
                    StatusCode = StatusCodes.Status500InternalServerError
                };
            }
        }
    }
}
