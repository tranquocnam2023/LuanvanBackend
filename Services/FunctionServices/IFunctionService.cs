using EMS_Backend.Dtos;
using EMS_Backend.Dtos.FunctionDtos;
using EMS_Backend.Dtos.PaginationDtos;

namespace EMS_Backend.Services.FunctionServices
{
    public interface IFunctionService
    {
        Task<ServiceResponse<FunctionResponse>> AddNewFunction(FunctionRequest functionRequest);
        Task<ServiceResponse<FunctionResponse>> GetFunctionById (string functionId);
        Task<ServiceResponse<PaginationResponse<FunctionResponse>>> GetAllFunctions (PaginationParams @params);
        Task<ServiceResponse<FunctionResponse>> UpdateFunction(FunctionRequest functionRequest);
        Task<ServiceResponse<FunctionResponse>> DeleteFunction(string functionId);
    }
}
