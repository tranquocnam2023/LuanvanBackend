namespace EMS_Backend.Entities
{
    public class RoleFunctions
    {
        public required string RoleId { get; set; }
        public required string FunctionId { get; set; }
        public bool IsActive { get; set; } = false;
        // Navigational properties
        public virtual Role? Role { get; set; }
        public virtual FunctionMaster? FunctionMaster { get; set; }
    }
}
