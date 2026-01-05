using Lpgin2.Models.Entities;

namespace Lpgin2.DTOs.Request.StudentMarkDTO
{
    public class UpdateStudentMarkDTO
    {
        public int EnrollmentId { get; set; }
        public Enums.enExamTypes ExamTypeId { get; set; }
        public decimal Mark { get; set; }
    }
}
