using CategoryMapper.FunctionApp.Data;
using CategoryMapper.FunctionApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CategoryMapper.FunctionApp.Services
{
    public interface ICategoryService
    {
        Task<List<GetCategoriesResponse>> GetCategoriesByParentAsync(Guid? parentId, int page, int size, bool includeProducts);
    }

    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<GetCategoriesResponse>> GetCategoriesByParentAsync(Guid? parentId, int page, int size, bool includeProducts)
        {
            var baseQuery = _context.Categories
                .Where(c => c.ParentId == parentId)
                .OrderBy(c => c.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(c => new GetCategoriesResponse
                {
                    Id = c.Id,
                    Name = c.Name,
                    HasChildren = _context.Categories.Any(child => child.ParentId == c.Id),
                    Products = includeProducts
                        ? _context.Products
                            .Where(p => p.CategoryId == c.Id)
                            .Select(p => new Product { Id = p.Id, Name = p.Name })
                            .ToList()
                        : new List<Product>()
                });

            return await baseQuery.ToListAsync();
        }
    }
}
