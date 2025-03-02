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
	public class AuthServiceTests
	{
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly JwtSettings _jwtSettings;
		private readonly AuthService.Services.AuthService _authService;

		public AuthServiceTests()
		{
			_userRepositoryMock = new Mock<IUserRepository>();

			// Setup sample JWT settings
			_jwtSettings = new JwtSettings
			{
				Secret = "TestSecretKey1234567890",
				ExpirationInMinutes = 60,
				Issuer = "TestIssuer",
				Audience = "TestAudience"
			};

			// Instantiate the service with mocks
			_authService = new AuthService.Services.AuthService(_userRepositoryMock.Object, _jwtSettings);
		}

		[Fact]
		public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
		{
			// Arrange
			var user = new User { Username = "existinguser", Email = "existing@example.com" };
			_userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("existinguser"))
							   .ReturnsAsync(user);

			// Act & Assert
			await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(user, "password"));
		}

		[Fact]
		public async Task RegisterAsync_ShouldReturnToken_WhenUserIsNew()
		{
			// Arrange
			var user = new User { Username = "newuser", Email = "new@example.com" };
			_userRepositoryMock.Setup(repo => repo.GetByUsernameAsync("newuser"))
							   .ReturnsAsync((User)null);
			_userRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<User>()))
							   .Returns(Task.CompletedTask);

			// Act
			var token = await _authService.RegisterAsync(user, "password");

			// Assert
			Assert.False(string.IsNullOrWhiteSpace(token));
		}

		[Fact]
		public async Task AuthenticateAsync_ShouldReturnToken_WhenCredentialsAreValid()
		{
			// Arrange
			string username = "testuser";
			string password = "password";
			var user = new User { Id = 1, Username = username, Email = "test@example.com" };

			// Simulate stored hash from our ComputeSha256Hash method.
			var sha256 = System.Security.Cryptography.SHA256.Create();
			var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			user.PasswordHash = BitConverter.ToString(bytes).Replace("-", "").ToLower();

			_userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
							   .ReturnsAsync(user);

			// Act
			var token = await _authService.AuthenticateAsync(username, password);

			// Assert
			Assert.False(string.IsNullOrWhiteSpace(token));
		}

		[Fact]
		public async Task AuthenticateAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
		{
			// Arrange
			string username = "testuser";
			string password = "password";
			var user = new User { Id = 1, Username = username, Email = "test@example.com", PasswordHash = "incorrecthash" };

			_userRepositoryMock.Setup(repo => repo.GetByUsernameAsync(username))
							   .ReturnsAsync(user);

			// Act
			var token = await _authService.AuthenticateAsync(username, password);

			// Assert
			Assert.Null(token);
		}
	}
}
