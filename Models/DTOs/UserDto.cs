namespace AuthService.Models.DTOs
{
    // This is the simplified blueprint for a User, used for output.
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; } // Concatenated first and last name
    }
}
