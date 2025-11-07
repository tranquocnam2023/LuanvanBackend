namespace EMS_Backend.Dtos.RoleFunctionDtos
{
    public class RoleFunctionRequest
    {
        public required string RoleId { get; set; }
        public required string FunctionId { get; set; }
        public bool IsActive { get; set;}
    }
}
