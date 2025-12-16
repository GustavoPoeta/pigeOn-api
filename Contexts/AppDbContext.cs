using Microsoft.EntityFrameworkCore;
using pigeon_api.Models;

namespace pigeon_api.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("users");
            modelBuilder.Entity<Friendship>()
                .ToTable("friendships");
            modelBuilder.Entity<Premium>()
                .ToTable("premium");
            modelBuilder.Entity<Message>()
                .ToTable("messages");
            modelBuilder.Entity<Notification>()
                .ToTable("notifications");

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<User> Users { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Premium> Premium { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
    }

}