using Lpgin2.Models.MTM;

namespace Lpgin2.Models.Entities
{
    public class clsCourse
    {
        public int id { set; get; }

        public int Hours { set; get; }

        public required string  CourseName { set; get; }

        public ICollection<clsCourseOffering> Offerings { set; get; } = new List<clsCourseOffering>();
    }
}
