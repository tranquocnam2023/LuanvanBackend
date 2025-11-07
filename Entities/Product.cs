namespace EMS_Backend.Entities
{
    public class Product
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public required float BasePrice { get; set; }
        public required int CategoryId { get; set; }
        public required string SupplierId { get; set; }
        public required string ThumbnailUrl { get; set; }
        public required bool IsVariant { get; set; }
        public virtual Category? Category { get; set; }
        public virtual Supplier? Supplier { get; set; }
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
        //public virtual ICollection<ProductVariants>? ProductVariants { get; set; }
    }
}
