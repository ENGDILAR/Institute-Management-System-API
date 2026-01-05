using Lpgin2.Models.Entities;

namespace Lpgin2.DTOs.Request
{
    public class StudentDTO
    {
        public required string FirstName { set; get; }
        public required string LastName { set; get; }
        public required string Address { set; get; }
        public required string Phone { set; get; }
        public required string Email { set; get; }
        public required string Password { set; get; }
        public string? EPhone { set; get; }
       

    }
}
