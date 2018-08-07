namespace Instagraph.Data
{
    using Microsoft.EntityFrameworkCore;
    using Instagraph.Models;

    public class InstagraphContext : DbContext
    {
        public InstagraphContext() { }

        public InstagraphContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserFollower> UsersFollowers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(Configuration.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserFollower>()
                .HasKey(uf => new { uf.UserId, uf.FollowerId });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(u => u.Followers)
                    .WithOne(f => f.User)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.UsersFollowing)
                    .WithOne(uf => uf.Follower)
                    .HasForeignKey(uf => uf.FollowerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Posts)
                    .WithOne(p => p.User)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Comments)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
