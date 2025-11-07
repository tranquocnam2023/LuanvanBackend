namespace EMS_Backend.Dtos.SupplierDtos
{
    public class SupplierRequest
    {
        public required string Id { get; set; }
        public required string SupplierName { get; set; }
        public string? SupplierAddress { get; set; }
        public string? SupplierPhone {  get; set; }
    }
}
