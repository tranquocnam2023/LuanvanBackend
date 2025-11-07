namespace EMS_Backend.Entities
{
    public class User
    {
        public required string UserId { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string RoleId { get; set; }

        // Navigational property
        public virtual Role? Role { get; set; }
    }
}