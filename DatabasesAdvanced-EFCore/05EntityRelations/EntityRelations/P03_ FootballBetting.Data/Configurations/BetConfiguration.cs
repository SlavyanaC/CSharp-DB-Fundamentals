namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;
    using System;

    public class BetConfiguration : IEntityTypeConfiguration<Bet>
    {
        public void Configure(EntityTypeBuilder<Bet> builder)
        {
            builder.ToTable("Bets");

            builder.HasKey(b => b.BetId);

            builder.Property(b => b.Amount)
                .IsRequired();

            builder.Property(b => b.Prediction)
                .IsRequired()
                .IsUnicode();

            builder.Property(b => b.DateTime)
                .HasDefaultValue(DateTime.Now);

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bets)
                .HasForeignKey(b => b.UserId);

            builder.HasOne(b => b.Game)
                .WithMany(g => g.Bets)
                .HasForeignKey(b => b.GameId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
