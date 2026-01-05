using Lpgin2.Models.Entities;
using static Lpgin2.Models.Entities.Enums;

namespace Lpgin2.DTOs.Request
{
    public class CourseOfferingDTO
    {
       
        public required int CourseId { set; get; }

        public required int DoctorId { set; get; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Capacity { get; set; } // سعة الطلاب
        public enCourseOfferingStatus Status { get; set; } = enCourseOfferingStatus.Open;
    }
}
