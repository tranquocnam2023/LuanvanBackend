using EMS_Backend.Dtos.RoleFunctionDtos;

namespace EMS_Backend.Dtos.RoleDtos
{
    public class RoleResponse
    {
        public required string RoleId { get; set; }
        public required string RoleName { get; set; }
        public List<RoleFunctionResponse> RoleFunctions { get; set; } = new();
    }
}
