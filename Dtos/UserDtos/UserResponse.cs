namespace EMS_Backend.Dtos.UserDtos
{
    public class UserResponse
    {
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleId { get; set; } = null!; 
        public string RoleName { get; set; } = null!;
    }
}
