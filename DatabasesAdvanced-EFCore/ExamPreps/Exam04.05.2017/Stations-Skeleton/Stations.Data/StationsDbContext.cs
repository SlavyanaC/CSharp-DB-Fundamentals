namespace Stations.Data
{
    using Microsoft.EntityFrameworkCore;
    using Stations.Models;

    public class StationsDbContext : DbContext
    {
        public StationsDbContext()
        {
        }

        public StationsDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<CustomerCard> Cards { get; set; }
        public DbSet<SeatingClass> SeatingClasses { get; set; }
        public DbSet<Station> Stations { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainSeat> TrainSeats { get; set; }
        public DbSet<Trip> Trips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SeatingClass>(entity =>
            {
                entity.HasIndex(sc => sc.Name)
                    .IsUnique();

                entity.HasIndex(sc => sc.Abbreviation)
                    .IsUnique();
            });

            modelBuilder.Entity<Station>(entity =>
            {
                entity.HasIndex(s => s.Name)
                    .IsUnique();

                entity.HasMany(s => s.TripsFrom)
                    .WithOne(t => t.OriginStation)
                    .HasForeignKey(t => t.OriginStationId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(s => s.TripsTo)
                    .WithOne(t => t.DestinationStation)
                    .HasForeignKey(t => t.DestinationStationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Train>(entity =>
            {
                entity.HasIndex(t => t.TrainNumber)
                    .IsUnique();

                entity.HasMany(t => t.Trips)
                    .WithOne(tr => tr.Train)
                    .HasForeignKey(tr => tr.TrainId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(t => t.CustomerCard)
                    .WithMany(pc => pc.BoughtTickets)
                    .HasForeignKey(t => t.CustomerCardId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}