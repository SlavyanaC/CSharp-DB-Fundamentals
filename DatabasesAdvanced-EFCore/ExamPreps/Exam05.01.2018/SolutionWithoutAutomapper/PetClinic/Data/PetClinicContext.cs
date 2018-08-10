namespace PetClinic.Data
{
    using Microsoft.EntityFrameworkCore;
    using PetClinic.Models;

    public class PetClinicContext : DbContext
    {
        public PetClinicContext() { }

        public PetClinicContext(DbContextOptions options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        public DbSet<Animal> Animals { get; set; }

        public DbSet<AnimalAid> AnimalAids { get; set; }

        public DbSet<Passport> Passports { get; set; }

        public DbSet<Procedure> Procedures { get; set; }

        public DbSet<ProcedureAnimalAid> ProcedureAnimalAids { get; set; }

        public DbSet<Vet> Vets { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ProcedureAnimalAid>(entity =>
            {
                entity.HasKey(e => new { e.ProcedureId, e.AnimalAidId });

                entity.HasOne(e => e.Procedure)
                    .WithMany(p => p.ProcedureAnimalAids)
                    .HasForeignKey(e => e.ProcedureId);

                entity.HasOne(e => e.AnimalAid)
                    .WithMany(ai => ai.AnimalAidProcedures)
                    .HasForeignKey(e => e.AnimalAidId);
            });

            builder.Entity<Animal>(entity =>
            {
                entity.HasMany(a => a.Procedures)
                    .WithOne(p => p.Animal)
                    .HasForeignKey(p => p.AnimalId);

                entity.HasOne(a => a.Passport)
                    .WithOne(p => p.Animal)
                    .HasForeignKey<Animal>(a => a.PassportSerialNumber);
            });

            builder.Entity<Vet>(entity =>
            {
                entity.HasMany(v => v.Proceduras)
                    .WithOne(p => p.Vet)
                    .HasForeignKey(p => p.VetId);

                entity.HasIndex(v => v.PhoneNumber)
                    .IsUnique();
            });

            builder.Entity<AnimalAid>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .IsUnique();
            });
        }
    }
}
