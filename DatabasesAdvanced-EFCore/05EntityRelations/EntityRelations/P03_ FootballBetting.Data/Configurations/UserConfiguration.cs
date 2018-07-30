namespace P03_FootballBetting.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using P03_FootballBetting.Data.Models;

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);

            builder.Property(u => u.Username)
                .IsRequired()
                .IsUnicode();

            builder.Property(u => u.Password)
                .IsRequired();

            builder.Property(u => u.Email)
                .IsRequired()
                .IsUnicode(false);

            builder.Property(u => u.Name)
                .IsRequired()
                .IsUnicode();
        }
    }
}
