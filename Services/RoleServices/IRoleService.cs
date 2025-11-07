using EMS_Backend.Dtos;
using EMS_Backend.Dtos.PaginationDtos;
using EMS_Backend.Dtos.RoleDtos;

namespace EMS_Backend.Services.RoleServices
{
    public interface IRoleService
    {
        Task<ServiceResponse<PaginationResponse<RoleResponse>>> GetAllRolesAsync(PaginationParams @params);
        Task<ServiceResponse<RoleResponse>> GetRoleByIdAsync(string roleId);
        Task<ServiceResponse<RoleResponse>> AddRoleAsync(RoleRequest request);
        Task<ServiceResponse<RoleResponse>> DeleteRoleAsync(string roleId);
        Task<ServiceResponse<RoleResponse>> UpdateRoleAsync (RoleRequest request);

    }
}
