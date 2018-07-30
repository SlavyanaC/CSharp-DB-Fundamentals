namespace ProductsShop.Data
{
    using Microsoft.EntityFrameworkCore;
    using ProductsShop.Models;

    public class ProductsShopContext : DbContext
    {
        public ProductsShopContext()
        {
        }

        public ProductsShopContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategoryProducts> CategoryProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConnectionConfig.ConnectionStrng);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(p => p.Seller)
                    .WithMany(u => u.ProductsSold)
                    .HasForeignKey(p => p.SellerId);

                entity.HasOne(p => p.Buyer)
                    .WithMany(u => u.ProductsBought)
                    .HasForeignKey(p => p.BuyerId);
            });

            modelBuilder.Entity<CategoryProducts>(entity =>
            {
                entity.HasKey(cp => new { cp.ProductId, cp.CategoryId });

                entity.HasOne(cp => cp.Category)
                    .WithMany(c => c.CategoryProducts)
                    .HasForeignKey(cp => cp.CategoryId);

                entity.HasOne(cp => cp.Product)
                    .WithMany(p => p.CategoryProducts)
                    .HasForeignKey(cp => cp.CategoryId);
            });
        }
    }
}
