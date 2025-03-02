using System;
using System.Threading.Tasks;
using AuthService.Helpers;
using AuthService.Models;
using AuthService.Repositories.Interfaces;
using AuthService.Services;
using Moq;
using Xunit;

namespace AuthService.Tests
{
    // Test class for AuthService
    public class AuthServiceTests
    {
        // Mock for the IUserRepository dependency
        private readonly Mock<IUserRepository> _userRepositoryMock;
        // Sample JWT settings used for testing token generation
        private readonly JwtSettings _jwtSettings;
        // Instance of the AuthService under test
        private readonly AuthService.Services.AuthService _authService;

        // Constructor initializes mocks and the service instance
        public AuthServiceTests()
        {
            // Initialize the mock repository
            _userRepositoryMock = new Mock<IUserRepository>();

            // Setup sample JWT settings with test values
            _jwtSettings = new JwtSettings
            {
                Secret = "TestSecretKey1234567890",
                ExpirationInMinutes = 60,
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            };

            // Instantiate the AuthService with the mock repository and test JWT settings
            _authService = new AuthService.Services.AuthService(_userRepositoryMock.Object, _jwtSettings);
        }

        // Test to ensure that registering an already existing user throws an exception.
        [Fact]
        public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange: Create a user object representing an already existing user.
            var user = new User { Username = "existinguser", Email = "existing@example.com" };
            // Setup the repository mock to return the user when queried by username.
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("existinguser"))
                               .ReturnsAsync(user);

            // Act & Assert: Verify that an exception is thrown when trying to register an existing user.
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(user, "password"));
        }

        // Test to ensure that registering a new user returns a valid JWT token.
        [Fact]
        public async Task RegisterAsync_ShouldReturnToken_WhenUserIsNew()
        {
            // Arrange: Create a new user object.
            var user = new User { Username = "newuser", Email = "new@example.com" };
            // Setup the repository mock to return null, indicating the user does not exist.
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("newuser"))
                               .ReturnsAsync((User)null);
            // Setup the repository mock to simulate user creation.
            _userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            // Act: Call the RegisterAsync method to register the new user.
            var token = await _authService.RegisterAsync(user, "password");

            // Assert: Check that a non-empty token string is returned.
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        // Test to ensure that authentication returns a valid token when credentials are valid.
        [Fact]
        public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange: Define valid username and password.
            string username = "testuser";
            string password = "password";
            // Create a user object with required properties.
            var user = new User { Id = 1, Username = username, Email = "test@example.com" };

            // Compute the SHA256 hash of the password to simulate a stored password hash.
            var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            user.PasswordHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            // Setup the repository mock to return the user when queried by username.
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                               .ReturnsAsync(user);

            // Act: Call the AuthenticateAsync method with valid credentials.
            var token = await _authService.AuthenticateAsync(username, password);

            // Assert: Verify that a non-empty token string is returned.
            Assert.False(string.IsNullOrWhiteSpace(token));
        }

        // Test to ensure that authentication returns null when credentials are invalid.
        [Fact]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange: Define username and password.
            string username = "testuser";
            string password = "password";
            // Create a user object with an incorrect stored password hash.
            var user = new User { Id = 1, Username = username, Email = "test@example.com", PasswordHash = "incorrecthash" };

            // Setup the repository mock to return the user with the wrong password hash.
            _userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
                               .ReturnsAsync(user);

            // Act: Attempt to authenticate with valid credentials but mismatched hash.
            var token = await _authService.AuthenticateAsync(username, password);

            // Assert: Verify that the authentication fails and returns null.
            Assert.Null(token);
        }
    }
}
