namespace CategoryMapper.FunctionApp.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public bool HasChildren => Children != null && Children.Count > 0;
        public List<Category> Children { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
    }

    public class GetCategoriesResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }

        public bool HasChildren;
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
