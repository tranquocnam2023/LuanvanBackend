namespace EMS_Backend.Services.ImageProductServices
{
    public interface IImageProductService
    {
        Task<string> UploadImageAsync(IFormFile imageStream, string fileName);
        Task<bool> DeleteImageAsync(string filePath);
        Task<bool> DeleteImageInFolder(string id);
        Task<string> UpdateImageAsync(IFormFile imageStream, string oldFilePath, string newFileName);
    }
}
