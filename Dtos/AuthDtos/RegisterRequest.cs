namespace EMS_Backend.Dtos.AuthDtos
{
    public class RegisterRequest
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
    }
}
