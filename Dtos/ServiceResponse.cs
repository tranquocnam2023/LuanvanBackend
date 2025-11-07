namespace EMS_Backend.Dtos
{
    public class ServiceResponse<T> where T : class
    {
        public T Data { get; set; } = null!;
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public string ErrorCode { get; set; } = string.Empty;
        public int StatusCode { get; set; } = StatusCodes.Status200OK;
    }
}
