using AuthService.Models;

namespace AuthService.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(User user, string password);
        Task<string> AuthenticateAsync(string username, string password);
    }
}
