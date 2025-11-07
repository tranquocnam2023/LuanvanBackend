namespace EMS_Backend.Dtos.AuthDtos
{
    public class AuthRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
