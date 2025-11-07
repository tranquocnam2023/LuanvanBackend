namespace EMS_Backend.Dtos.CategoryDtos
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = null!;
        public int ParentCategoryId { get; set; }
        public List<CategoryResponse> childCates { get; set; } = new List<CategoryResponse>();
    }
}
