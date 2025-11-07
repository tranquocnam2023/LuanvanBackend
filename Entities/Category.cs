namespace EMS_Backend.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        //public virtual ICollection<AttributeGroup>? AttributeGroups { get; set; }
        //public virtual ICollection<CategoryAttributeTemplate>? CategoryAttributeTemplates { get; set; }
    }
}