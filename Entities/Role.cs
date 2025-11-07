namespace EMS_Backend.Entities
{
    public class Role
    {
        public required string RoleId { get; set; }
        public required string RoleName { get; set; }

        // Navigational property
        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<RoleFunctions>? RoleFunctions { get; set; }
    }
}
