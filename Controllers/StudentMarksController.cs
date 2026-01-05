using Humanizer;
using Lpgin2.Data;
using Lpgin2.DTOs.Request.StudentMarkDTO;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentMarksController : ControllerBase
    {
        private readonly AppDBContext _context;
        public StudentMarksController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("AddNewMark",Name ="AddNewMark")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<AddStudentMarkDTO>>AddNewMark(AddStudentMarkDTO smdto)
        
        {
            var enrollment =await _context.enrollments.FindAsync(smdto.EnrollmentId);
            if (enrollment == null)
                return BadRequest($"There is No Enrollment with ID{smdto.EnrollmentId}");

            var existingMarks = await _context.StudentMarks
            .Where(m => m.EnrollmentId == smdto.EnrollmentId) .ToListAsync();

            if (existingMarks.Any(m => m.ExamTypeId == (int)smdto.ExamTypeId))
                return Conflict("This exam mark already exists. You can update it instead.");

            switch (smdto.ExamTypeId)
            {
                case Enums.enExamTypes.Secound:
                    if (!existingMarks.Any(m => m.ExamTypeId == (int)Enums.enExamTypes.First))
                        return BadRequest("Cannot add second exam mark before first exam mark.");
                    break;
                case Enums.enExamTypes.Final:
                    if (!existingMarks.Any(m => m.ExamTypeId == (int)Enums.enExamTypes.First) ||
                        !existingMarks.Any(m => m.ExamTypeId == (int)Enums.enExamTypes.Secound))
                        return BadRequest("Cannot add final exam mark before first and second exam marks.");
                    break;
            }

            var newMark = new clsStudentMark
            {
                EnrollmentId = smdto.EnrollmentId,
                ExamTypeId = (int)smdto.ExamTypeId,
                Mark = smdto.Mark
            };

            _context.StudentMarks.Add(newMark);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Mark added successfully" });
        }

        [HttpPatch("UpdateMark")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<StudentMarkResponseDTO>>UpdateMark(UpdateStudentMarkDTO smdto)
        {
            var mark = await _context.StudentMarks.FirstOrDefaultAsync(
                m => m.EnrollmentId == smdto.EnrollmentId &&
                m.ExamTypeId == (int)smdto.ExamTypeId
                );
        if(mark==null)
                return NotFound("Mark not found for this enrollment and exam type.");

            mark.Mark = smdto.Mark;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Mark updated successfully" });
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpGet("GetAllMarksByCourseOfferingID{COID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetAllMarksByCourseOfferingID(int COID)
        {
            if (COID <= 0)
                return BadRequest("Please Enter a Valid ID");
            var marks = await _context.StudentMarks.Where(sm => sm.Enrollment.CourseOfferingId == COID)
                .Select(sm => new StudentMarksForCourseDTO
                {
                    StudentId = sm.Enrollment.StudentId,
                    ExamType = ((Enums.enExamTypes)sm.ExamTypeId).ToString(),
                    StudentName = (sm.Enrollment.Student.FirstName + " " + sm.Enrollment.Student.LastName),
                    Mark = sm.Mark
                }).ToListAsync();

            if(!marks.Any())
            {
                return NoContent();
            }

            return Ok(marks);
        }


        [HttpGet("GetMarksByStudentID{StudentID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task <IActionResult>GetMArksByStudentID(int StudentID)
        {
            if (StudentID <= 0)
                return BadRequest("Please Enter A valid Student ID");
            var marks = await _context.StudentMarks.Where(sm => sm.Enrollment.StudentId == StudentID)
                .Select(
                m => new
                {
                    CourseName = m.Enrollment.CourseOffering.Course.CourseName,
                    ExampType = ((Enums.enExamTypes)m.ExamTypeId).ToString(),
                    Mark = m.Mark

                }).ToListAsync();
             
            if(!marks.Any())
            {
                return NoContent();
            }

            return Ok(marks);
        }



    }

}
