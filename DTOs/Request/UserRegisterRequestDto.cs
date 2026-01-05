using System.ComponentModel.DataAnnotations;

namespace Lpgin2.DTOs.Request
{
    public class UserRegisterRequestDto
    {
        [Required(ErrorMessage = "Email is required")]
        public required string  Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }
    }
}
