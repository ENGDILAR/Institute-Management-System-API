using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.Entities;
using Lpgin2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly AppDBContext _context;
        public StudentsController(AppDBContext context)
        {
            _context = context;
        }

        [HttpPost("AddNewStudents", Name = "AddNewStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddNewStudents([FromBody] StudentDTO student)
        {
            if (student == null)
                return BadRequest("Invalid student data.");

    
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
               
                if (await _context.users.AnyAsync(u => u.Email == student.Email))
                    return Conflict("Email already exists. Please use another email.");

                var newUser = new clsUser
                {
                    Email = student.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(student.Password),
                    role = Enums.enUserRoles.student
                };

                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                var newStudent = new clsStudent
                {
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Address = student.Address,
                    Phone = student.Phone,
                    EPhone = student.EPhone,
                    UserId = newUser.id
                };

                _context.students.Add(newStudent);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "User registered successfully",
                    student = new StudentResopnseDTO
                    {
                        studentID = newStudent.id,
                        UserID = newUser.id,
                        Email = newUser.Email,
                        FirstName = newStudent.FirstName,
                        LastName = newStudent.LastName,
                        Address = newStudent.Address,
                        Phone = newStudent.Phone,
                        EPhone = newStudent.EPhone
                    }
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                return Conflict("Concurrency conflict occurred. Please try again.");
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();
                // إذا كان السبب وجود بريد إلكتروني مكرر (في حالة حدوث تعارض متزامن)
                if (dbEx.InnerException != null && dbEx.InnerException.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
                    return Conflict("Email already exists (duplicate detected).");

                return StatusCode(StatusCodes.Status500InternalServerError, "Database error occurred.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error: {.Message}");
            }
        }

        [HttpPatch("UpdateStudent")]
        public async Task<ActionResult<StudentResopnseDTO>> UpdateStudent(int studentID, [FromBody] StudentDTO student)
        {
            if (studentID <= 0)
                return BadRequest($"The ID: {studentID} is not valid.");

            var std = await _context.students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == studentID);

            if (std == null)
                return NotFound($"No student found with ID {studentID}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ تحقق من البريد الإلكتروني الجديد فقط إذا تم إدخاله وكان مختلفًا
                if (!string.IsNullOrWhiteSpace(student.Email) &&
                    std.User != null &&
                    std.User.Email != student.Email)
                {
                    bool emailExists = await _context.users.AnyAsync(u => u.Email == student.Email && u.id != std.User.id);
                    if (emailExists)
                        return Conflict("Email already exists for another user.");

                    std.User.Email = student.Email;
                }

                // ✅ تحديث الحقول فقط إذا أرسل المستخدم قيمًا جديدة
                if (!string.IsNullOrWhiteSpace(student.FirstName))
                    std.FirstName = student.FirstName;

                if (!string.IsNullOrWhiteSpace(student.LastName))
                    std.LastName = student.LastName;

                if (!string.IsNullOrWhiteSpace(student.Address))
                    std.Address = student.Address;

                if (!string.IsNullOrWhiteSpace(student.Phone))
                    std.Phone = student.Phone;

                if (!string.IsNullOrWhiteSpace(student.EPhone))
                    std.EPhone = student.EPhone;

            
                if (!string.IsNullOrWhiteSpace(student.Password) && std.User != null)
                    std.User.Password = BCrypt.Net.BCrypt.HashPassword(student.Password);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Student updated successfully",
                    student = new StudentResopnseDTO
                    {
                        studentID = std.id,
                        UserID = std.UserId,
                        Email = std.User?.Email!,
                        FirstName = std.FirstName,
                        LastName = std.LastName,
                        Address = std.Address,
                        Phone = std.Phone,
                        EPhone = std.EPhone
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Update failed: {ex.Message}");
            }
        }
        [HttpGet("AllStudents")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult<IEnumerable<clsStudent>>> GetAllStudents()
        {
            var students = await _context.students
                .Include(s => s.User)
                .ToListAsync();
            if (students.Count == 0)
                return NoContent();

            return Ok(
                students.Select(s=>new StudentResopnseDTO 
                {
                    studentID = s.id,
                    UserID = s.UserId,
                    Email = s.User.Email,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Address = s.Address,
                    Phone = s.Phone,
                    EPhone = s.EPhone
                })
                );
        }


        [HttpGet("GetStudentByID/{StudentID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentResopnseDTO>> GetStudentByID(int StudentID)
        {
            if (StudentID <= 0)
                return BadRequest("Please Insert Valied ID Between {1 ,,, 1000}");

            var std = await _context.students
                .Include(s=>s.User)
                .FirstOrDefaultAsync(s=>s.id==StudentID);
            if (std == null)
            {
                return NotFound($"There is no Student with ID {StudentID}");
            }

          
            return Ok( new StudentResopnseDTO
            { 
                 Address = std.Address,
                 Email= std.User.Email,
                 EPhone = std.EPhone,
                 Phone=std.Phone,
                 FirstName=std.FirstName,
                 LastName=std.LastName,
                 studentID=std.id,
                 UserID=std.UserId
            });
        }


        [HttpDelete("DeleteStudent/{StudentID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStudent(int StudentID)
        {
            if (StudentID <= 0)
                return BadRequest($"The ID: {StudentID} is not valid.");

            var student = await _context.students
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == StudentID);

            if (student == null)
                return NotFound($"There is no student with ID {StudentID}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (student.User != null)
                    _context.users.Remove(student.User);

                _context.students.Remove(student);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return NoContent();
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting student.");
            }
        }

    }
}
