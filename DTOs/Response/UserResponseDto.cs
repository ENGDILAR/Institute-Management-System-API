using Lpgin2.Models.Entities;

namespace Lpgin2.DTOs.Response
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }  // مثل: Admin, User, Manager ...
        public required string  Token { get; set; } // JWT Token
        public required bool IsSuccess { get; set; }
        public required string Message { get; set; }
        public required string RedirectUrl { get; set; } // لتوجيه المستخدم للوحة المناسبة
    }
}
