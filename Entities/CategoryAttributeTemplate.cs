namespace EMS_Backend.Entities
{
    public class CategoryAttributeTemplate
    {
        public int TemplateId { get; set; }
        public int CategoryId { get; set; }
        public required string AttributeId { get; set; }
        public bool isRequired { get; set; } = false;
        public bool isVariant { get; set; } = false;
        public bool isFilterable { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public virtual Category? Category { get; set; }
        public virtual Attribute? Attribute { get; set; }
    }
}
