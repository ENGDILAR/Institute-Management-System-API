using Lpgin2.Models.Entities;
using Lpgin2.Models.MTM;

namespace Lpgin2.DTOs.Response
{
    public class StudentMarkResponseDTO
    {
        public int StudentMarkId { get; set; }
        public int EnrollmentId { get; set; }

        public int ExamTypeId { get; set; }

        public decimal Mark { get; set; }
    }
}
