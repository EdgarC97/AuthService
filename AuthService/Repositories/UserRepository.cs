using AuthService.Data;
using AuthService.Models;
using AuthService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.Repositories
{
    // The UserRepository is our contractor that interacts directly with the database warehouse.
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Retrieves a user by their unique ID.
        public async Task<User> GetByIdAsync(int id)
        {
            // Uses the context to find the user by id asynchronously.
            return await _context.Users.FindAsync(id);
        }

        // Retrieves a user by their username.
        public async Task<User> GetByUsernameAsync(string username)
        {
            // Searches the Users table for a record with a matching username (case-insensitive).
            return await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        // Retrieves all users from the database.
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            // Converts the Users table to a list asynchronously.
            return await _context.Users.ToListAsync();
        }

        // Creates a new user in the database.
        public async Task CreateAsync(User user)
        {
            // Adds the new user to the Users table asynchronously.
            await _context.Users.AddAsync(user);
            // Persists the changes to the database.
            await _context.SaveChangesAsync();
        }

        // Updates an existing user's information.
        public async Task UpdateAsync(User user)
        {
            // Marks the user entity as modified in the context.
            _context.Users.Update(user);
            // Persists the changes to the database.
            await _context.SaveChangesAsync();
        }

        // Deletes a user by their unique ID.
        public async Task DeleteAsync(int id)
        {
            // Retrieves the user using the GetByIdAsync method.
            var user = await GetByIdAsync(id);
            // Checks if the user exists before attempting deletion.
            if (user != null)
            {
                // Removes the user from the Users table.
                _context.Users.Remove(user);
                // Persists the changes to the database.
                await _context.SaveChangesAsync();
            }
        }
    }
}
