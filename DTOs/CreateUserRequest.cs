using System.ComponentModel.DataAnnotations;

namespace role_play.DTOs
{
    public class CreateUserRequest
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FullName { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        public string Role { get; set; } = "User";
        public bool IsVerified { get; set; }

        public string OTPCode { get; set; } = null!;
        public DateTime OTPExpiry { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
