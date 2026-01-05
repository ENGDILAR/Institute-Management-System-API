using Lpgin2.Models.Entities;
using System.ComponentModel.DataAnnotations;
using static Lpgin2.Models.Entities.Enums;

namespace Lpgin2.Models.MTM
{
    public class clsCourseOffering
    {
        public int Id { set; get; }
        public required int CourseId { set; get; }
        public clsCourse? Course { set; get; }
        public required int DoctorId { set; get; }
        public clsDoctor? doctor { set; get; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Capacity { get; set; } // سعة الطلاب
        public enCourseOfferingStatus Status { get; set; } = enCourseOfferingStatus.Open;
        public ICollection<clsEnrollments> Enrollments { get; set; } = new List<clsEnrollments>();

    }
}
