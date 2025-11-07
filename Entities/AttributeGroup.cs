namespace EMS_Backend.Entities
{
    public class AttributeGroup
    {
        public required string GroupId { get; set; }
        public required string GroupName { get; set; }
        public int CategoryId { get; set; }
        public required int SortOrder { get; set; } = 0;
        public virtual Category? Category { get; set; }
        public virtual ICollection<Attribute>? Attributes { get; set; }
    }
}
