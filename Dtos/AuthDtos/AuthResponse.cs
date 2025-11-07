namespace EMS_Backend.Dtos.AuthDtos
{
    public class AuthResponse
    {
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string RoleId { get; set; } = null!;
    }
}
