using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuthService.Models.DTOs;
using AuthService.Services.Interfaces;

namespace AuthService.Controllers
{
    // Indicates that this class is an API controller.
    [ApiController]
    // Defines the base route for this controller. "[controller]" is replaced by the controller name "Users".
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // Private read-only field to store the user service dependency.
        private readonly IUserService _userService;

        // Constructor with dependency injection to supply an instance of IUserService.
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Users
        // Endpoint to retrieve all users.
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            // Calls the user service to asynchronously fetch all users.
            var users = await _userService.GetAllUsersAsync();

            // Returns a successful response (HTTP 200) with the list of users.
            return Ok(users);
        }

        // GET: api/Users/{id}
        // Endpoint to retrieve a specific user by their ID.
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            // Calls the user service to fetch a user by ID.
            var user = await _userService.GetUserByIdAsync(id);

            // If the user is not found, returns a NotFound response (HTTP 404).
            if (user == null)
                return NotFound();

            // Returns a successful response (HTTP 200) with the user's data.
            return Ok(user);
        }

        // PUT: api/Users/{id}
        // Endpoint to update an existing user's details.
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            // Calls the user service to update the user with the provided data.
            var updatedUser = await _userService.UpdateUserAsync(id, updateUserDto);

            // If the user is not found, returns a NotFound response (HTTP 404).
            if (updatedUser == null)
                return NotFound();

            // Returns a successful response (HTTP 200) with the updated user data.
            return Ok(updatedUser);
        }

        // DELETE: api/Users/{id}
        // Endpoint to delete a user by their ID.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Calls the user service to delete the user.
            var result = await _userService.DeleteUserAsync(id);

            // If deletion fails (user not found), returns a NotFound response (HTTP 404).
            if (!result)
                return NotFound();

            // Returns a No Content response (HTTP 204) indicating successful deletion.
            return NoContent();
        }
    }
}