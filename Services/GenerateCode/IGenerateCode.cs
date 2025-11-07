namespace EMS_Backend.Services.GenerateCode
{
    public interface IGenerateCode
    {
        Task<string> GenerateCodeAsync(string prefix, string table, string column);
    }
}
