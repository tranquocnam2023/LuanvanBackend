namespace EMS_Backend.Dtos.CategoryDtos
{
    public class CategoryRequest
    {
        public int? Id { get; set; }
        public required string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}
