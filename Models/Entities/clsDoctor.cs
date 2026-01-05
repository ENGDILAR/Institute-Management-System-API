using Lpgin2.Models.MTM;

namespace Lpgin2.Models.Entities
{
    public class clsDoctor
    {
        public int id { set; get; }
        public required string FirstName { set; get; }
        public required string LastName { set; get; }
       public int UserId { set; get; }
        public required string Address { set; get; }
        public required string Phone { set; get; }
        public string? EPhone { set; get; }
        public clsUser User { get; set; } = null!;

        public ICollection<clsCourseOffering> Offerings { set; get; } = new List<clsCourseOffering>();
    }
}
