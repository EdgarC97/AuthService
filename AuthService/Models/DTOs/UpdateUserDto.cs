namespace AuthService.Models.DTOs
{
    // This DTO is used when a client wants to update an existing User.
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        // Optionally include password change functionality
        public string Password { get; set; }
    }
}
