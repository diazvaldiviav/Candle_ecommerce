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
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<Aroma> Aromas { get; set; }
    public DbSet<ProductAroma> ProductAromas { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

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

        modelBuilder.Entity<ProductColor>()
       .HasKey(pc => new { pc.ProductId, pc.ColorId });

        modelBuilder.Entity<ProductSize>()
            .HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSizes)
            .HasForeignKey(ps => ps.ProductId);

        modelBuilder.Entity<ProductSize>()
            .HasOne(ps => ps.Size)
            .WithMany(s => s.ProductSizes)
            .HasForeignKey(ps => ps.SizeId);

        modelBuilder.Entity<ProductSize>()
       .HasKey(ps => new { ps.ProductId, ps.SizeId });


        // Configuración de ProductImage
        modelBuilder.Entity<ProductImage>()
            .HasOne(pi => pi.Product)
            .WithMany(p => p.ProductImages)
            .HasForeignKey(pi => pi.ProductId)
            .IsRequired(false);  // Hace la relación opcional

        // Configuración de Review
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .IsRequired(false);

      //  modelBuilder.Entity<Review>()
         //   .HasOne(r => r.User)
           // .WithMany()
           // .HasForeignKey(r => r.UserId)
           // .IsRequired(false);

        // Configuración de ProductAroma
        modelBuilder.Entity<ProductAroma>()
            .HasKey(pa => new { pa.ProductId, pa.AromaId });

        modelBuilder.Entity<ProductAroma>()
            .HasOne(pa => pa.Product)
            .WithMany(p => p.ProductAromas)
            .HasForeignKey(pa => pa.ProductId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<ProductAroma>()
      .HasOne(pa => pa.Aroma)
      .WithMany(a => a.ProductAromas)
      .HasForeignKey(pa => pa.AromaId)
      .OnDelete(DeleteBehavior.Cascade);

        // Configuraciones adicionales
        modelBuilder.Entity<Product>()
            .Property(p => p.Name)
            .IsRequired(false);

        modelBuilder.Entity<Product>()
            .Property(p => p.Description)
            .IsRequired(false);

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Aroma>()
            .Property(a => a.Name)
            .IsRequired(false);

        modelBuilder.Entity<Aroma>()
            .Property(a => a.Description)
            .IsRequired(false);

        // Configuración de precisión decimal para Coupon
        modelBuilder.Entity<Coupon>()
            .Property(c => c.DiscountAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Coupon>()
            .Property(c => c.MinimumPurchase)
            .HasPrecision(18, 2);

        // Configuración de precisión decimal para ProductAroma
        modelBuilder.Entity<ProductAroma>()
            .Property(pa => pa.AdditionalPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Coupon>()
            .Property(c => c.Code)
            .IsRequired(false);

        modelBuilder.Entity<UserRole>()
       .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configuración de Cart
        modelBuilder.Entity<Cart>()
            .HasOne(c => c.User)
            .WithMany(u => u.Carts)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

       

        // Actualizar la configuración de Review para incluir la relación con User
        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .IsRequired(false);

        // Configuraciones adicionales para User
        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Password)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.FirstName)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.LastName)
            .IsRequired();



        modelBuilder.Entity<CartItem>(entity =>
        {
            // Configurar la tabla
            entity.ToTable("CartItems");

            // Configurar la clave primaria
            entity.HasKey(e => e.Id);

            // Configurar las propiedades
            entity.Property(e => e.Id)
                .UseIdentityColumn();

            entity.Property(e => e.CartId)
                .IsRequired();

            entity.Property(e => e.ProductId)
                .IsRequired();

            entity.Property(e => e.Quantity)
                .IsRequired();

            entity.Property(e => e.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();

            // Configurar las relaciones
            entity.HasOne(d => d.Cart)
                .WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Color)
                .WithMany()
                .HasForeignKey(d => d.ColorId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Size)
                .WithMany()
                .HasForeignKey(d => d.SizeId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Aroma)
                .WithMany()
                .HasForeignKey(d => d.AromaId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        });



    }
}
