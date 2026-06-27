namespace Lpgin2.Models.Entities
{
    public class clsAdmin
    {
        public int id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
        public string? EPhone { get; set; }

        public int UserId { get; set; }
        public clsUser User { get; set; } = null!;

    }
}
