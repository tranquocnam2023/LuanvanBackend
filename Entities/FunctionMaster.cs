namespace EMS_Backend.Entities
{
    public class FunctionMaster
    {
        public required string FunctionId { get; set; }
        public required string FunctionName { get; set; }
        public virtual ICollection<RoleFunctions>? RoleFunctions { get; set; }
    }
}
