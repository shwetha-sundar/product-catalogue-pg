namespace CategoryMapper.FunctionApp.Models
{
    public class CategoryAttributeLink
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid AttributeId { get; set; }
        public string LinkType { get; set; } // direct or inherited

        public virtual Category Category { get; set; }
        public virtual Attribute Attribute { get; set; }
    }
}
