using FastFood.Models;
using Microsoft.EntityFrameworkCore;

namespace FastFood.Data
{
    public class FastFoodDbContext : DbContext
    {
        public FastFoodDbContext() { }

        public FastFoodDbContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Position> Positions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            if (!builder.IsConfigured)
            {
                builder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Position>()
                .HasAlternateKey(e => e.Name);

            builder.Entity<Employee>()
                .HasOne(e => e.Position)
                .WithMany(p => p.Employees)
                .HasForeignKey(e => e.PositionId);

            builder.Entity<Item>(entity =>
            {
                entity.HasAlternateKey(e => e.Name);

                entity.HasOne(i => i.Category)
                    .WithMany(c => c.Items)
                    .HasForeignKey(i => i.CategoryId);
            });

            builder.Entity<Order>()
                .HasOne(i => i.Employee)
                .WithMany(c => c.Orders)
                .HasForeignKey(i => i.EmployeeId);

            builder.Entity<OrderItem>(entity =>
             {
                 entity.HasKey(e => new { e.OrderId, e.ItemId });

                 entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId);

                 entity.HasOne(e => e.Item)
                    .WithMany(i => i.OrderItems)
                    .HasForeignKey(e => e.ItemId);
             });
        }
    }
}