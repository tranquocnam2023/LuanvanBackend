using Supabase;
using Supabase.Storage;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
namespace EMS_Backend.Services.ImageProductServices
{
    public class ImageProductService : IImageProductService
    {
        private readonly Supabase.Client _supabaseClient;
        private readonly string _bucketName;
        public ImageProductService(IConfiguration configuration)
        {
            _supabaseClient = new Supabase.Client(
                        configuration["BucketCloudImage:Url"],
                        configuration["BucketCloudImage:AnonKey"]
                    );
            _bucketName = configuration["BucketCloudImage:BucketName"] ?? "product-bucket";
        }

        public async Task<bool> DeleteImageAsync(string filePath)
        {
            //throw new NotImplementedException();
            try
            {
                await _supabaseClient.Storage.From(_bucketName).Remove(new List<string> { filePath });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteImageInFolder(string id)
        {
            //throw new NotImplementedException();
            try
            {
                var listImageInFolder = await _supabaseClient.Storage.From(_bucketName).List(id);
                if (listImageInFolder == null || listImageInFolder.Any() == false)
                {
                    return false;
                }

                var filesToDelete = listImageInFolder.Select(file => $"{id}/{file.Name}").ToList();

                await _supabaseClient.Storage.From(_bucketName).Remove(filesToDelete);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> UpdateImageAsync(IFormFile imageStream, string oldFilePath, string newFileName)
        {
            await DeleteImageAsync(oldFilePath);
            return await UploadImageAsync(imageStream, newFileName);
        }

        public async Task<string> UploadImageAsync(IFormFile imageStream, string fileName)
        {
            //    throw new NotImplementedException();
            try
            {
                var storage = _supabaseClient.Storage.From(_bucketName);

                using var ms = new MemoryStream();
                await imageStream.CopyToAsync(ms);
                var bytes = ms.ToArray();

                var result = await storage.Upload(bytes, fileName, new Supabase.Storage.FileOptions
                {
                    CacheControl = "3600",
                    Upsert = true
                });

                // Lấy URL public
                var publicUrl = storage.GetPublicUrl(fileName);
                return publicUrl;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
