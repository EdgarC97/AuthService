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
    // Test class for the AuthController
    public class AuthControllerTests
    {
        // Mock instance for the IAuthService dependency
        private readonly Mock<IAuthService> _authServiceMock;
        // Instance of the AuthController that will be tested
        private readonly AuthController _authController;

        // Constructor to initialize the mock and controller instances
        public AuthControllerTests()
        {
            // Create a new mock for IAuthService
            _authServiceMock = new Mock<IAuthService>();
            // Inject the mocked IAuthService into the AuthController
            _authController = new AuthController(_authServiceMock.Object);
        }

        // Test to verify that the Register endpoint returns an Ok result containing a token when registration is successful
        [Fact]
        public async Task Register_ReturnsOkResult_WithToken()
        {
            // Arrange: Create a sample register request object with user data.
            var registerRequest = new RegisterRequest
            {
                Username = "newuser",
                Email = "new@example.com",
                Password = "password",
                FirstName = "John",
                LastName = "Doe"
            };

            // Arrange: Setup the mock to return a sample token when RegisterAsync is called with any User and password.
            _authServiceMock.Setup(service => service.RegisterAsync(
                    It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync("sample-token");

            // Act: Call the Register method on the controller with the test register request.
            var result = await _authController.Register(registerRequest);

            // Assert: Check if the result is of type OkObjectResult.
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Assert: Verify that the returned value contains the expected token.
            Assert.Contains("sample-token", okResult.Value.ToString());
        }

        // Test to verify that the Login endpoint returns an Ok result with a token when valid credentials are provided.
        [Fact]
        public async Task Login_ReturnsOkResult_WithToken_WhenCredentialsAreValid()
        {
            // Arrange: Create a sample login request with valid credentials.
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "password"
            };

            // Arrange: Setup the mock to return a sample token when AuthenticateAsync is called with the valid credentials.
            _authServiceMock.Setup(service => service.AuthenticateAsync(
                    loginRequest.Username, loginRequest.Password))
                .ReturnsAsync("sample-token");

            // Act: Call the Login method on the controller with the test login request.
            var result = await _authController.Login(loginRequest);

            // Assert: Check if the result is of type OkObjectResult.
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Assert: Verify that the returned value contains the expected token.
            Assert.Contains("sample-token", okResult.Value.ToString());
        }

        // Test to verify that the Login endpoint returns an Unauthorized result when credentials are invalid.
        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange: Create a sample login request with invalid credentials.
            var loginRequest = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            // Arrange: Setup the mock to return null when AuthenticateAsync is called with invalid credentials.
            _authServiceMock.Setup(service => service.AuthenticateAsync(
                    loginRequest.Username, loginRequest.Password))
                .ReturnsAsync((string)null);

            // Act: Call the Login method on the controller with the invalid login request.
            var result = await _authController.Login(loginRequest);

            // Assert: Verify that the result is of type UnauthorizedObjectResult.
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}
