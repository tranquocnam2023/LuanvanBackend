using EMS_Backend.Dtos.RoleFunctionDtos;

namespace EMS_Backend.Dtos.RoleDtos
{
    public class RoleRequest
    {
        public required string RoleId { get; set; }
        public required string RoleName { get; set; }
        public List<RoleFunctionRequest> roleFunctionRequests { get; set; } = new();
    }
}
