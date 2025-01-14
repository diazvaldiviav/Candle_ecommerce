// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Candle_API.Data.Entities;

public class CandleDbContext : DbContext
{
    public CandleDbContext(DbContextOptions<CandleDbContext> options)
        : base(options)
    {
    }


    // Tablas

    public DbSet<Category> Categories { get; set; }
    public DbSet<SubCategory> Subcategories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<ProductColor> ProductColors { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones de relaciones
        modelBuilder.Entity<SubCategory>()
            .HasOne(s => s.Category)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(s => s.CategoryId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.SubCategory)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SubcategoryId);

        modelBuilder.Entity<ProductColor>()
            .HasOne(pc => pc.Product)
            .WithMany(p => p.ProductColors)
            .HasForeignKey(pc => pc.ProductId);

        modelBuilder.Entity<ProductColor>()
            .HasOne(pc => pc.Color)
            .WithMany(c => c.ProductColors)
            .HasForeignKey(pc => pc.ColorId);

        modelBuilder.Entity<ProductSize>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSizes)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSize>()
            .HasOne(ps => ps.Size)
            .WithMany(s => s.ProductSizes)
            .HasForeignKey(ps => ps.SizeId);
    }
}
