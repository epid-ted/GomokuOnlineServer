using MatchServer.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MatchServer.Web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserStamina> UserStamina { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            builder.Entity<UserStamina>()
                .HasIndex(us => us.UserId)
                .IsUnique();
        }
    }
}
