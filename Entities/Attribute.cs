namespace EMS_Backend.Entities
{
    public class Attribute
    {
        public required string AttributeId { get; set; }
        public required string AttributeName { get; set; }
        public required string AttributeGroupId { get; set; }
        public string? AttributeType { get; set; }
        public virtual AttributeGroup? AttributeGroup { get; set; }
        public virtual ICollection<AttributeOption>? AttributeOptions { get; set; }
        public virtual ICollection<CategoryAttributeTemplate>? CategoryAttributeTemplates { get; set; }
    }
}
