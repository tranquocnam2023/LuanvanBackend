namespace EMS_Backend.Entities
{
    public class Supplier
    {
        public required string Id { get; set; }
        public required string SupplierName { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhone { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
    }
}
