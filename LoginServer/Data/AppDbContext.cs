using LoginServer.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoginServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
