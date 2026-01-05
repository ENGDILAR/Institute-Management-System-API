using Lpgin2.Models.MTM;

namespace Lpgin2.Models.Entities
{
    public class clsStudent
    {
        public  int id { set; get; }
        public required string FirstName { set; get; }
        public required string LastName { set; get; }

        public required string Address { set; get; }
        public required string Phone { set; get; }
        public  string? EPhone { set; get; }

        // 🔹 المفتاح الأجنبي (Foreign Key)

        public int UserId { get; set; }

        // 🔹 العلاقة مع المستخدم
        public clsUser User { get; set; } = null!;

        public ICollection<clsEnrollments> enrollments { set; get; } = new List<clsEnrollments>();
    }
}
