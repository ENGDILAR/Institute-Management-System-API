using Lpgin2.Models.Entities;

namespace Lpgin2.DTOs.Request.StudentMarkDTO
{
    public class StudentMarksForCourseDTO
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public decimal Mark { get; set; }
    }
}
