using Microsoft.EntityFrameworkCore;
using AuthService.Models;

namespace AuthService.Data
{
    // The ApplicationDbContext acts as our connection to the database warehouse.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Each DbSet is like a room blueprint stored in the warehouse.
        public DbSet<User> Users { get; set; }
    }
}
