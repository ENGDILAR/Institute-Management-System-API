using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.MTM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly AppDBContext _context;

        public EnrollmentController(AppDBContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,doctor")]
        [HttpGet("GetEnrollmentsByOfferID/{courseOfferId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EnrollmentResponseDTO>>> GetAllEnrollmentsByOfferId(int courseOfferId)
        {
            var enrollments = await _context.enrollments
                .Where(e => e.CourseOfferingId == courseOfferId)
                .ToListAsync();

            if (!enrollments.Any())
                return NotFound("No enrollments found for this course offering.");

            var result = enrollments.Select(e => new EnrollmentResponseDTO
            {
                Id = e.Id,
                CourseOfferingId = e.CourseOfferingId,
                StudentId = e.StudentId,
                RegisteredAt = e.RegisteredAt
            });

            return Ok(result);
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetEnrollmentsByStudentID/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<EnrollmentResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<EnrollmentResponseDTO>>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _context.enrollments
                .Where(e => e.StudentId == studentId)
                .ToListAsync();

            if (!enrollments.Any())
                return NotFound($"No enrollments found for student with ID {studentId}.");

            var result = enrollments.Select(e => new EnrollmentResponseDTO
            {
                Id = e.Id,
                CourseOfferingId = e.CourseOfferingId,
                StudentId = e.StudentId,
                RegisteredAt = e.RegisteredAt
            });

            return Ok(result);
        }


        [HttpGet("GetEnrollmentByID/{id}")]
        [ProducesResponseType( StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnrollmentResponseDTO>> GetEnrollmentByID(int id)
        {
            var enrollment = await _context.enrollments
                .FirstOrDefaultAsync(e => e.Id == id);

            if (enrollment == null)
                return NotFound($"Enrollment with ID {id} not found.");

            var response = new EnrollmentResponseDTO
            {
                Id = enrollment.Id,
                CourseOfferingId = enrollment.CourseOfferingId,
                StudentId = enrollment.StudentId,
                RegisteredAt = enrollment.RegisteredAt
            };

            return Ok(response);
        }

        [HttpPost("AddNewEnrollment", Name = "CreateEnrollment")]
        [ProducesResponseType( StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<EnrollmentResponseDTO>> CreateEnrollment(EnrollmentDTO enrollment)
        {
            if (enrollment == null || enrollment.CourseOfferingId <= 0 || enrollment.StudentId <= 0)
                return BadRequest("Invalid enrollment data. Please check again.");

            var courseOffering = await _context.courseOfferings
        .FirstOrDefaultAsync(o => o.Id == enrollment.CourseOfferingId);
            if (courseOffering == null)
                return NotFound($"Course offering with ID {enrollment.CourseOfferingId} not found.");

            var studentExists = await _context.students
                .AnyAsync(s => s.id == enrollment.StudentId);
            if (!studentExists)
                return NotFound($"Student with ID {enrollment.StudentId} not found.");

            bool alreadyEnrolled = await _context.enrollments
                .AnyAsync(e => e.CourseOfferingId == enrollment.CourseOfferingId && e.StudentId == enrollment.StudentId);
            if (alreadyEnrolled)
                return Conflict("The student is already enrolled in this course offering.");

            if (courseOffering.Capacity <= 0)
                return Conflict("No available seats in this course offering.");
            var entity = new clsEnrollments
            {
                CourseOfferingId = enrollment.CourseOfferingId,
                RegisteredAt = enrollment.RegisteredAt,
                StudentId = enrollment.StudentId
            };

          

            await _context.enrollments.AddAsync(entity);
            courseOffering.Capacity -= 1;

            await _context.SaveChangesAsync();

            var response = new EnrollmentResponseDTO
            {
                Id = entity.Id,
                CourseOfferingId = entity.CourseOfferingId,
                StudentId = entity.StudentId,
                RegisteredAt = entity.RegisteredAt
            };

           
            return CreatedAtRoute("GetEnByID", new { id = entity.Id }, response);
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("DeleteInrollmentByID{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.enrollments.FindAsync(id);

            if (enrollment == null)
                return NotFound($"Enrollment with ID {id} not found.");

            var courseOffering = await _context.courseOfferings
      .FirstOrDefaultAsync(o => o.Id == enrollment.CourseOfferingId);

            if (courseOffering != null)
            {
                
                courseOffering.Capacity += 1;
            }
            _context.enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            return Ok("The Enrollment Has Been Deleted Succefully");
        }


    }
}
