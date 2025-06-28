using CategoryMapper.FunctionApp.Models;
using Microsoft.EntityFrameworkCore;
using Attribute = CategoryMapper.FunctionApp.Models.Attribute;

namespace CategoryMapper.FunctionApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Attribute> Attributes { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryAttributeLink> CategoryAttributeLinks { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasKey(c => c.Id);
            modelBuilder.Entity<Attribute>().HasKey(a => a.Id);
            modelBuilder.Entity<CategoryAttributeLink>().HasKey(cal => cal.Id);
            modelBuilder.Entity<Product>().HasKey(p => p.Id);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Children)
                .WithOne()
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Category>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId);
        }
    }
}
