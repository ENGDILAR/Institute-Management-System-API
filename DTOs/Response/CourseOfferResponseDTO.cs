using static Lpgin2.Models.Entities.Enums;

namespace Lpgin2.DTOs.Response
{
    public class CourseOfferResponseDTO
    {
       public required int OfferId { set; get; }
        public required int CourseId { set; get; }

        public required int DoctorId { set; get; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Capacity { get; set; }
        public enCourseOfferingStatus Status { get; set; } = enCourseOfferingStatus.Open;
    }
}
