namespace AuthService.Models.DTOs
{
    // DTO for user registration requests.
    public class RegisterRequest
    {
        // Username provided by the user.
        public string Username { get; set; }
        // Email address provided by the user.
        public string Email { get; set; }
        // Password provided by the user.
        public string Password { get; set; }
        // User's first name.
        public string FirstName { get; set; }
        // User's last name.
        public string LastName { get; set; }
    }

    // DTO for user login requests.
    public class LoginRequest
    {
        // Username used for authentication.
        public string Username { get; set; }
        // Password used for authentication.
        public string Password { get; set; }
    }
}
