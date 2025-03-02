using System.Threading.Tasks;
using AuthService.Controllers;
using AuthService.Models;
using AuthService.Models.DTOs;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AuthService.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _authController = new AuthController(_authServiceMock.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkResult_WithToken()
        {
            // Arrange
            var registerRequest = new RegisterRequest
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password",
                FirstName = "John",
                LastName = "Doe"
            };

            // Setup mock to return a token
            _authServiceMock.Setup(service => service.RegisterAsync(
                    It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync("sample-token");

            // Act
            var result = await _authController.Register(registerRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("sample-token", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WithToken_WhenCredentialsAreValid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password"
            };

            _authServiceMock.Setup(service => service.AuthenticateAsync(
                    loginRequest.Username, loginRequest.Password))
                .ReturnsAsync("sample-token");

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("sample-token", okResult.Value.ToString());
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            _authServiceMock.Setup(service => service.AuthenticateAsync(
                    loginRequest.Username, loginRequest.Password))
                .ReturnsAsync((string)null);

            // Act
            var result = await _authController.Login(loginRequest);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
