using System.ComponentModel.DataAnnotations;

namespace role_play.DTOs
{
    public class ResendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
