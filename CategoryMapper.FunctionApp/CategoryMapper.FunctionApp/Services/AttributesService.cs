using CategoryMapper.FunctionApp.Data;
using Microsoft.EntityFrameworkCore;
using Attribute = CategoryMapper.FunctionApp.Models.Attribute;

namespace CategoryMapper.FunctionApp.Services
{
    public interface IAttributeService
    {
        Task<List<Attribute>> GetAttributesAsync(
            List<Guid> categoryIds,
            List<string> linkTypes,
            string keyword,
            bool notApplicable,
            int page,
            int size);
    }

    public class AttributeService : IAttributeService
    {
        private readonly AppDbContext _context;

        public AttributeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Attribute>> GetAttributesAsync(
            List<Guid> categoryIds,
            List<string> linkTypes,
            string keyword,
            bool notApplicable,
            int page,
            int size)
        {
            IQueryable<Attribute> attributeQuery = _context.Attributes.AsQueryable();

            if (categoryIds.Any())
            {
                var directOrInheritedLinks = _context.CategoryAttributeLinks
                    .Where(cal => categoryIds.Contains(cal.CategoryId));

                var nonGlobalTypes = linkTypes.Where(lt => lt != "global").ToList();
                if (nonGlobalTypes.Any())
                {
                    directOrInheritedLinks = directOrInheritedLinks
                        .Where(cal => nonGlobalTypes.Contains(cal.LinkType));
                }

                var applicableAttributeIds = await directOrInheritedLinks
                        .Select(cal => cal.AttributeId)
                        .Distinct()
                        .ToListAsync();

                bool includeGlobalAttributes = linkTypes.Contains("global") && !notApplicable;

                if (includeGlobalAttributes)
                {
                    var globalAttributeIds = await _context.Attributes
                        .Where(attr => !_context.CategoryAttributeLinks.Any(link => link.AttributeId == attr.Id))
                        .Select(attr => attr.Id)
                        .ToListAsync();

                    applicableAttributeIds.AddRange(globalAttributeIds);
                }

                attributeQuery = notApplicable ? 
                    attributeQuery.Where(attr => !applicableAttributeIds.Contains(attr.Id))
                    : attributeQuery.Where(attr => applicableAttributeIds.Contains(attr.Id));
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                attributeQuery = attributeQuery.Where(a => a.Name.Contains(keyword));
            }

            return await attributeQuery
                .OrderBy(a => a.Name)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();
        }
    }
}
