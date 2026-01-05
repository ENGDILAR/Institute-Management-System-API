using Lpgin2.Models.Entities;
using Lpgin2.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Models.MTM
{
    public class clsEnrollments
    {
        public int Id { set; get; }
        public required int StudentId { set; get; }
        public clsStudent Student { set; get; } = null!;
        public required int  CourseOfferingId { get; set; }
        public clsCourseOffering CourseOffering { get; set; } = null!;
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public ICollection<clsStudentMark>? StudentMarks { get; set; } = new List<clsStudentMark>();
    }

}
