using AuthService.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AuthService.Repositories.Interfaces
{
    // The IUserRepository interface defines the contractor's responsibilities for managing users.
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task<IEnumerable<User>> GetAllAsync();
        Task CreateAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
    }
}
