using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Helpers;
using AuthService.Models;
using AuthService.Repositories.Interfaces;
using AuthService.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthService.Services
{
    // Implements the authentication service functionality.
    public class AuthService : IAuthService
    {
        // Repository for accessing user data.
        private readonly IUserRepository _userRepository;
        // Settings for generating JWT tokens.
        private readonly JwtSettings _jwtSettings;

        // Constructor with dependency injection for IUserRepository and JwtSettings.
        public AuthService(IUserRepository userRepository, JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings;
        }

        // Registers a new user with the provided password.
        public async Task<string> RegisterAsync(User user, string password)
        {
            // Check if a user with the same username already exists.
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
                // If user exists, throw an exception indicating duplication.
                throw new Exception("User already exists.");

            // Compute and assign the hashed password for security.
            user.PasswordHash = ComputeSha256Hash(password);
            // Create the new user record in the repository.
            await _userRepository.CreateAsync(user);
            // Generate and return a JWT token for the newly registered user.
            return GenerateToken(user);
        }

        // Authenticates a user by validating their credentials.
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            // Retrieve the user by username.
            var user = await _userRepository.GetByUsernameAsync(username);
            // Validate if the user exists and the provided password hash matches.
            if (user == null || user.PasswordHash != ComputeSha256Hash(password))
                // Return null if authentication fails.
                return null;
            // Generate and return a JWT token if authentication succeeds.
            return GenerateToken(user);
        }

        // Generates a JWT token for the given user.
        private string GenerateToken(User user)
        {
            // Create a new token handler instance.
            var tokenHandler = new JwtSecurityTokenHandler();
            // Convert the secret key from settings to a byte array.
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            // Create a list of claims with the user's username and identifier.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            // Define token properties like subject, expiration, issuer, audience, and signing credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the token based on the descriptor.
            var token = tokenHandler.CreateToken(tokenDescriptor);
            // Write and return the token string.
            return tokenHandler.WriteToken(token);
        }

        // Computes the SHA256 hash of the given raw data (password).
        private string ComputeSha256Hash(string rawData)
        {
            // Create a new SHA256 instance to compute the hash.
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash as a byte array.
                var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                // Convert the byte array to a hexadecimal string and return it.
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }
    }
}
