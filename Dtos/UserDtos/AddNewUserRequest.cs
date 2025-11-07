namespace EMS_Backend.Dtos.UserDtos
{
    public class AddNewUserRequest
    {
        public required string FullName { get; set; }
        public required string RoleId { get; set; }
    }
}
