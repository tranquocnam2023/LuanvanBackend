namespace EMS_Backend.Entities
{
    public class ProductVariants
    {
        public int VariantId { get; set; }
        public required string ProductId { get; set; }
        public required string SKU { get; set; }
        public required float CurrentPrice { get; set; }
        public required float OldPrice { get; set; }
        public bool IsActive { get; set; } = false;
        public virtual Product? Product { get; set; }
    }
}
