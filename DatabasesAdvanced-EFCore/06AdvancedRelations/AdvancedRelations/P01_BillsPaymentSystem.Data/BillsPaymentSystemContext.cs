namespace P01_BillsPaymentSystem.Data
{
    using Microsoft.EntityFrameworkCore;
    using P01_BillsPaymentSystem.Data.Configurations;
    using P01_BillsPaymentSystem.Data.Models;

    public class BillsPaymentSystemContext : DbContext
    {
        public BillsPaymentSystemContext()
        {
        }

        public BillsPaymentSystemContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<BankAccount> BankAccounts { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(DbContextConfiguration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BankAccountConfiguration());
            modelBuilder.ApplyConfiguration(new CreditCardConfiguration());
        }
    }
}
