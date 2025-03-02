// Import necessary namespaces containing models, service interfaces, and MVC functionalities.
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using AuthService.Models.DTOs;

namespace AuthService.Controllers
{
    // Indicates that this class is an API controller and enables automatic model validation and response formatting.
    [ApiController]
    // Defines the base route for this controller. "[controller]" is replaced by the controller name "Auth".
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Private read-only field to store the authentication service dependency.
        private readonly IAuthService _authService;

        // Constructor with dependency injection to supply an instance of IAuthService.
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        // Endpoint to register a new user.
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                // Create a new user object from the request data. Only required properties are assigned.
                var user = new User
                {
                    Username = request.Username,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                // Calls the RegisterAsync method on the auth service to create the user and generate a token,
                // passing the newly created user and the password provided in the request.
                var token = await _authService.RegisterAsync(user, request.Password);

                // Returns a successful response (HTTP 200) with the generated token.
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                // Returns a Bad Request (HTTP 400) with the error message in case of an exception.
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/Auth/login
        // Endpoint to authenticate an existing user and generate a token.
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Calls the AuthenticateAsync method with the username and password provided in the request.
            var token = await _authService.AuthenticateAsync(request.Username, request.Password);

            // If no token is returned, it indicates invalid credentials; respond with Unauthorized (HTTP 401).
            if (token == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // Returns a successful response (HTTP 200) with the generated token.
            return Ok(new { token });
        }
    }
}