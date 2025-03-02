using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace AuthService.Data
{
    // SeedData acts like initial furnishing for our building.
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // Look for any users.
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }

                context.Users.AddRange(
                    new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        PasswordHash = "adminhashed", // In a real scenario, hash the password
                        FirstName = "Admin",
                        LastName = "User",
                        CreatedAt = DateTime.UtcNow
                    },
                    new User
                    {
                        Username = "john_doe",
                        Email = "john.doe@example.com",
                        PasswordHash = "johnhashed",
                        FirstName = "John",
                        LastName = "Doe",
                        CreatedAt = DateTime.UtcNow
                    }
                );
                context.SaveChanges();
            }
        }
    }
}
