namespace EMS_Backend.Dtos.UserDtos
{
    public class UserRequest
    {
        public required string Username { get; set; }
        public string? Password { get; set; }
        public required string FullName { get; set; }
        public required string RoleId { get; set; }
    }
}
