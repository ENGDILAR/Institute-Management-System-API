using Lpgin2.Models.Entities;
using Lpgin2.Models.MTM;

namespace Lpgin2.DTOs.Request.StudentMarkDTO
{
    public class AddStudentMarkDTO
    {
        public int EnrollmentId { get; set; }    
        public Enums.enExamTypes ExamTypeId { get; set; }
        public decimal Mark { get; set; }
    }
}
