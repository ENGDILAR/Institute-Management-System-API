using Lpgin2.Models.MTM;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Models.Entities
{
    public class clsStudentMark
    {
      
        public int Id { get; set; }
        public int EnrollmentId { get; set; }
        public clsEnrollments Enrollment { get; set; } = null!;

        public int ExamTypeId { get; set; }
        public clsExamType ExamType { get; set; } = null!;

        public decimal Mark { get; set; }
    }
}
