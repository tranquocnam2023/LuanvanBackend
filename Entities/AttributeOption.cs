namespace EMS_Backend.Entities
{
    public class AttributeOption
    {
        public required string OptionId { get; set; }
        public required string OptionValue { get; set; }
        public required string AttributeId { get; set; }
        public bool IsActive { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public virtual Attribute? Attribute { get; set; }
    }
}
