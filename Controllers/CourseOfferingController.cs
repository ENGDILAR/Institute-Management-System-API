using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.MTM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseOfferingController : ControllerBase
    {
        private readonly AppDBContext _context;
        public CourseOfferingController(AppDBContext context)
        {
            _context = context;
        }

        
        [HttpPost("AddNewOffer", Name = "AddNewOffer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewOffer([FromBody] CourseOfferingDTO dto)
        {
         
            bool courseExists = await _context.courses.AnyAsync(c => c.id == dto.CourseId);
            bool doctorExists = await _context.doctors.AnyAsync(d => d.id == dto.DoctorId);
            if (!courseExists || !doctorExists)
                return BadRequest("Course or Doctor not found.");

            var offering = new clsCourseOffering
            {
                CourseId = dto.CourseId,
                DoctorId = dto.DoctorId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Capacity = dto.Capacity,
                Status = dto.Status
            };

            _context.courseOfferings.Add(offering);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOfferByID), new { id = offering.Id }, offering);
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetOfferByID/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOfferByID(int id)
        {
            var offering = await _context.courseOfferings
                .Include(o => o.Course)
                .Include(o => o.doctor)
                .Include(o => o.Enrollments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offering == null) return NotFound();

            var isFull = offering.Enrollments.Count >= offering.Capacity;

            var OfferDTO = new CourseOfferResponseDTO
            {
                Capacity = offering.Capacity,
                CourseId = offering.CourseId,
                DoctorId = offering.DoctorId,
                OfferId = offering.Id,
                Status = offering.Status,
                EndDate = offering.EndDate,
                StartDate = offering.StartDate
            };
            return Ok(new { OfferDTO, isFull });
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetAllCourseOfferings")]
        public async Task<ActionResult<IEnumerable<CourseOfferResponseDTO>>> GetAllCourseOfferings()
        {
            var Offerings = await _context.courseOfferings
                .Include(c => c.doctor)
                .Include(c => c.Enrollments)
                .Include(c => c.Course)
                .ToListAsync();

            if (Offerings.Count == 0)
                return NotFound("There are no offerings in the system.");

            return Ok(
                Offerings.Select(o => new CourseOfferResponseDTO
                {
                     OfferId = o.Id,
                    Capacity = o.Capacity,
                    CourseId = o.CourseId,
                    DoctorId = o.DoctorId,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    Status = o.Status
                })
            );
        }

       
        [HttpPut("UpdateOffer/{id}")]
        public async Task<IActionResult> UpdateOffer(int id, [FromBody] CourseOfferingDTO dto)
        {
            var offering = await _context.courseOfferings.FindAsync(id);
            if (offering == null) return NotFound("Offer not found.");

            offering.CourseId = dto.CourseId;
            offering.DoctorId = dto.DoctorId;
            offering.StartDate = dto.StartDate;
            offering.EndDate = dto.EndDate;
            offering.Capacity = dto.Capacity;
            offering.Status = dto.Status;

            _context.courseOfferings.Update(offering);
            await _context.SaveChangesAsync();

            return Ok(offering);
        }

        
        [HttpDelete("DeleteOffer/{id}")]
        public async Task<IActionResult> DeleteOffer(int id)
        {
            var offering = await _context.courseOfferings
                .Include(o => o.Enrollments)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (offering == null) return NotFound("Offer not found.");

    
            if (offering.Enrollments.Any())
                _context.enrollments.RemoveRange(offering.Enrollments);

            _context.courseOfferings.Remove(offering);
            await _context.SaveChangesAsync();

            return Ok("The Offer Has Been Deleted Succefully");
        }
    }
}
