using System.ComponentModel.DataAnnotations;

namespace role_play.DTOs
{
    public class VerifyEmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(6, MinimumLength = 6)]
        public string OTPCode { get; set; } = null!;
    }
}
