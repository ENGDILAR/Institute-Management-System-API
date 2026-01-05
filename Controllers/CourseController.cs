using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
      private readonly AppDBContext _context;

        public CourseController(AppDBContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetAllCourses")]
        public async Task<ActionResult<IEnumerable<CourseResponseDTO>>> GetAllCourses()
        {
            var course = await _context.courses.ToListAsync();

            if (course.Count == 0)
                return NotFound("There are no students in the system.");

            return Ok(
                course.Select(c => new CourseResponseDTO
                {
                   CourseId = c.id,
                   CourseName = c.CourseName,
                   Hours = c.Hours
                })
                );
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]       
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseResponseDTO>> GetCourseById(int id)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound($"No course found with ID: {id}");

            var dto = new CourseResponseDTO
            {
                 CourseId = course.id,
                CourseName = course.CourseName,
                Hours = course.Hours
            };

            return Ok(dto);
        }

        [Authorize(Roles = "admin,student")]
        [HttpGet("GetByName")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseResponseDTO>> GetCourseByName([FromQuery] string name)
        {
            var course = await _context.courses
                .FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());

            if (course == null)
                return NotFound($"No course found with name: {name}");

            var dto = new CourseResponseDTO
            {
                 CourseId = course.id,
                CourseName = course.CourseName,
                Hours = course.Hours
            };

            return Ok(dto);
        }


     
        [HttpPost("AddCourse")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CourseResponseDTO>> AddCourse([FromBody] CourseDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var course = new clsCourse
            {
                CourseName = model.CourseName,
                Hours = model.Hours
            };

            _context.courses.Add(course);
            await _context.SaveChangesAsync();

            var response = new CourseResponseDTO
            {
                 CourseId  = course.id,
                CourseName = course.CourseName,
                Hours = course.Hours
            };

            return CreatedAtAction(nameof(GetCourseById), new { id = course.id }, response);
        }

   
        [HttpPut("Update/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDTO model)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound($"No course found with ID: {id}");

            course.CourseName = model.CourseName;
            course.Hours = model.Hours;

            await _context.SaveChangesAsync();

            return Ok($"Course with ID {id} updated successfully");
        }

     
        [HttpDelete("Delete/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.courses.FindAsync(id);
            if (course == null)
                return NotFound($"No course found with ID: {id}");

            _context.courses.Remove(course);
            await _context.SaveChangesAsync();

            return Ok($"Course with ID {id} deleted successfully");
        }
    }

}
