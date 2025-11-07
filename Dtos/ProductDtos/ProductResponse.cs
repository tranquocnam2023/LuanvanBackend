namespace EMS_Backend.Dtos.ProductDtos
{
    public class ProductResponse
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required float BasePrice { get; set; }
        public required int CateId { get; set; }
        public required string CateName { get; set; }
        public required string SupplierId { get; set; }
        public required string SupplierName { get; set; }
        public required string ThumbnailUrl { get; set; }
    }
}
