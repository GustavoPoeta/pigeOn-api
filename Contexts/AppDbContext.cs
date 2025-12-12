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

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<User> Users { get; set; }
    }

}