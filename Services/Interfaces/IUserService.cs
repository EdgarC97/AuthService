using AuthService.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AuthService.Services.Interfaces
{
    // The IUserService defines the project manager’s tasks in coordinating user-related business logic.
    public interface IUserService
    {
        Task<UserDto> RegisterAsync(CreateUserDto createUserDto);
        Task<UserDto> AuthenticateAsync(string username, string password);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> UpdateUserAsync(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(int id);
    }
}
