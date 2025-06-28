using CategoryMapper.FunctionApp;
using CategoryMapper.FunctionApp.Data;
using CategoryMapper.FunctionApp.Models;
using CategoryMapper.FunctionApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CategoryMapper.Tests
{
    [TestClass]
    public class CategoryServiceTests
    {
        private AppDbContext CreateTestDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var root = new Category { Id = Guid.NewGuid(), Name = "Root", ParentId = null };
            var child1 = new Category { Id = Guid.NewGuid(), Name = "Child1", ParentId = root.Id };
            var child2 = new Category { Id = Guid.NewGuid(), Name = "Child2", ParentId = root.Id };
            var product1 = new Product { Id = Guid.NewGuid(), Name = "Phone", CategoryId = root.Id };
            var product2 = new Product { Id = Guid.NewGuid(), Name = "Tablet", CategoryId = child1.Id };

            context.Categories.AddRange(root, child1, child2);
            context.Products.AddRange(product1, product2);
            context.SaveChanges();

            return context;
        }

        [TestMethod]
        public async Task GetRootCategories_ReturnsOnlyRootLevel()
        {
            var context = CreateTestDb();
            var service = new CategoryService(context);

            var result = await service.GetCategoriesByParentAsync(null, 1, 10, includeProducts: false);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Root", result[0].GetType().GetProperty("Name")?.GetValue(result[0]));
        }

        [TestMethod]
        public async Task GetChildCategories_ByParentId_ReturnsCorrectChildren()
        {
            var db = CreateTestDb();
            var service = new CategoryService(db);

            var rootId = db.Categories.First(c => c.Name == "Root").Id;

            var result = await service.GetCategoriesByParentAsync(rootId, 1, 10, includeProducts: false);

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Child1", "Child2" },
                result.Select(r => r.GetType().GetProperty("Name")?.GetValue(r)?.ToString()).ToList()
            );
        }

        [TestMethod]
        public async Task IncludeProducts_ReturnsProductsList()
        {
            var db = CreateTestDb();
            var service = new CategoryService(db);

            var result = await service.GetCategoriesByParentAsync(null, 1, 10, includeProducts: true);

            var firstCategory = result[0];
            var products = firstCategory.GetType().GetProperty("Products")?.GetValue(firstCategory) as List<Product>;

            Assert.IsNotNull(products);
            Assert.AreEqual(1, products.Count);
        }

        [TestMethod]
        public async Task InvalidParentId_ReturnsEmpty()
        {
            var db = CreateTestDb();
            var service = new CategoryService(db);

            var result = await service.GetCategoriesByParentAsync(Guid.NewGuid(), 1, 10, includeProducts: true);

            Assert.AreEqual(0, result.Count);
        }
    }
}