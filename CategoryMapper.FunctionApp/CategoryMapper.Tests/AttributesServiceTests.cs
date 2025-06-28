using CategoryMapper.FunctionApp.Data;
using CategoryMapper.FunctionApp.Models;
using CategoryMapper.FunctionApp.Services;
using Microsoft.EntityFrameworkCore;
using Attribute = CategoryMapper.FunctionApp.Models.Attribute;

namespace CategoryMapper.Tests
{
    [TestClass]
    public class AttributeServiceTests
    {
        private AppDbContext CreateTestDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new AppDbContext(options);

            var attr1 = new Attribute { Id = Guid.NewGuid(), Name = "Color" };
            var attr2 = new Attribute { Id = Guid.NewGuid(), Name = "Size" };
            var attr3 = new Attribute { Id = Guid.NewGuid(), Name = "Weight" };

            var cat1 = new Category { Id = Guid.NewGuid(), Name = "Clothing" };
            var link1 = new CategoryAttributeLink { Id = Guid.NewGuid(), CategoryId = cat1.Id, AttributeId = attr1.Id, LinkType = "direct" };
            var link2 = new CategoryAttributeLink { Id = Guid.NewGuid(), CategoryId = cat1.Id, AttributeId = attr2.Id, LinkType = "inherited" };

            context.Attributes.AddRange(attr1, attr2, attr3);
            context.Categories.Add(cat1);
            context.CategoryAttributeLinks.AddRange(link1, link2);
            context.SaveChanges();

            return context;
        }

        [TestMethod]
        public async Task GetAttributes_WithDirectAndInherited_ReturnsCorrectAttributes()
        {
            var db = CreateTestDbContext();
            var service = new AttributeService(db);
            var categoryId = db.Categories.First().Id;

            var result = await service.GetAttributesAsync(
                new List<Guid> { categoryId },
                new List<string> { "direct", "inherited" },
                null, false, 1, 10);

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Color", "Size" },
                result.Select(r => r.Name).ToList());
        }

        [TestMethod]
        public async Task GetAttributes_WithNotApplicable_ReturnsGlobalAttributes()
        {
            var db = CreateTestDbContext();
            var service = new AttributeService(db);
            var categoryId = db.Categories.First().Id;

            var result = await service.GetAttributesAsync(
                new List<Guid> { categoryId },
                new List<string> { "direct", "inherited", "global" },
                null, true, 1, 10);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Weight", result[0].Name); // Not linked to category
        }

        [TestMethod]
        public async Task GetAttributes_WithKeywordFilter_ReturnsMatching()
        {
            var db = CreateTestDbContext();
            var service = new AttributeService(db);

            var result = await service.GetAttributesAsync(
                new(), new(), "Size", false, 1, 10);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Size", result[0].Name);
        }
    }
}
