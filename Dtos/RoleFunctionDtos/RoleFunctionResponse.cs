namespace EMS_Backend.Dtos.RoleFunctionDtos
{
    public class RoleFunctionResponse
    {
        public required string FunctionId { get; set; }
        public required string FunctionName { get; set; }
        public bool IsActive { get; set; }
    }
}
