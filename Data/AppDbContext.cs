using Microsoft.EntityFrameworkCore;
using CoachClone.Api.Models;

namespace CoachClone.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        // Senare: public DbSet<JournalFile> JournalFiles { get; set; }
    }
}
