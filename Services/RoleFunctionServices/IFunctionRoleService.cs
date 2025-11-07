using EMS_Backend.Dtos.RoleFunctionDtos;

namespace EMS_Backend.Services.RoleFunctionServices
{
    public interface IFunctionRoleService
    {
        Task<List<RoleFunctionResponse>> CreateOrUpdateRoleFunction(List<RoleFunctionRequest> roleFunctionRequests);
        Task<List<RoleFunctionResponse>> GetAllRoleFunction();
        Task<List<RoleFunctionResponse>> GetRoleFunctionsWithRoleId(string roleId);
        Task<bool> DeleteRoleFunction(string roleId);
    }
}
