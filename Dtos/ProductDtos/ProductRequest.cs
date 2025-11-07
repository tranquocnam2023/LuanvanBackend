namespace EMS_Backend.Dtos.ProductDtos
{
    public class ProductRequest
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required float BasePrice { get; set; }
        public required int CategoryId { get; set; }
        public required string SupplierId { get; set; }
        public required IFormFile fileImageThumbnail { get; set; }
        public required bool IsVariant { get; set; }
    }
}
