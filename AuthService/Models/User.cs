namespace AuthService.Models
{
    // This is the detailed blueprint of a User in our building (system).
    public class User
    {
        public int Id { get; set; } // Unique room number
        public string? Username { get; set; } // Room label (must be unique)
        public string Email { get; set; } // Contact information
        public string? PasswordHash { get; set; } // Secure lock code for the room
        public string FirstName { get; set; } // Additional feature: first name
        public string LastName { get; set; }  // Additional feature: last name
        public DateTime CreatedAt { get; set; } // Date when the room was built (registered)
        public DateTime? UpdatedAt { get; set; } // Last update timestamp
    }
}
