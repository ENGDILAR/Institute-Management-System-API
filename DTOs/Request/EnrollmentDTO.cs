using Lpgin2.Models.Entities;
using Lpgin2.Models.MTM;

namespace Lpgin2.DTOs.Request
{
    public class EnrollmentDTO
    {
       
        public required int StudentId { set; get; }
        public required int CourseOfferingId { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
    }
}
