namespace P01_BillsPaymentSystem.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class BankAccountConfiguration : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.HasOne(b => b.PaymentMethod)
                .WithOne(p => p.BankAccount)
                .HasForeignKey<PaymentMethod>(p => p.BankAccountId);
        }
    }
}
