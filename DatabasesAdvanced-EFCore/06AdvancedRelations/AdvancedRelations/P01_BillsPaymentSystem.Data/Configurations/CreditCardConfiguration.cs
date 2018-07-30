namespace P01_BillsPaymentSystem.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P01_BillsPaymentSystem.Data.Models;

    public class CreditCardConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            builder.HasOne(c => c.PaymentMethod)
                .WithOne(p => p.CreditCard)
                .HasForeignKey<PaymentMethod>(p => p.CreditCardId);
        }
    }
}
