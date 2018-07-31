namespace CarDealer.Data
{
    using Microsoft.EntityFrameworkCore;
    using CarDealer.Models;

    public class CarDealerContext : DbContext
    {
        public CarDealerContext()
        {
        }

        public CarDealerContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PartCars> PartCars { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbConnetionConfig.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PartCars>(entity =>
            {
                entity.HasKey(pc => new { pc.Part_Id, pc.Car_Id });

                entity.HasOne(pc => pc.Part)
                    .WithMany(p => p.PartCars)
                    .HasForeignKey(pc => pc.Part_Id);

                entity.HasOne(pc => pc.Car)
                    .WithMany(c => c.PartCars)
                    .HasForeignKey(pc => pc.Car_Id);
            });

            modelBuilder.Entity<Sale>(entity =>
            {
                entity.HasOne(s => s.Customer)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.Customer_Id);

                entity.HasOne(s => s.Car)
                    .WithMany(c => c.Sales)
                    .HasForeignKey(s => s.Car_Id);
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.HasMany(s => s.Parts)
                    .WithOne(p => p.Supplier)
                    .HasForeignKey(p => p.Supplier_Id);
            });
        }
    }
}
