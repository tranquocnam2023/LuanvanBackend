namespace EMS_Backend.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public required string ProductId { get; set; }
        public string? ImageProductElement { get; set; }
        public virtual Product? Product { get; set; }
    }
}
