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
    public class DoctorController : ControllerBase
    {
        private readonly AppDBContext _context;
       

        public DoctorController(AppDBContext context)
        {
            _context = context;
       
        }


        [HttpPost("AddNewDoctor", Name = "AddNewDoctor")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddNewDoctor([FromBody] DoctorDTO doctor)
        {
            if (doctor == null)
                return BadRequest("Invalid Doctor data.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // التحقق من وجود البريد الإلكتروني قبل الإدراج
                if (await _context.users.AnyAsync(u => u.Email == doctor.Email))
                    return Conflict("Email already exists. Please use another email.");

                var newUser = new clsUser
                {
                    Email = doctor.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(doctor.Password),
                    role = Enums.enUserRoles.doctor
                };

                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                var newDoctor = new clsDoctor
                {
                    FirstName = doctor.FirstName,
                    LastName = doctor.LastName,
                    Address = doctor.Address,
                    Phone = doctor.Phone,
                    EPhone = doctor.EPhone,
                    UserId  = newUser.id
                };

                _context.doctors.Add(newDoctor);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "User registered successfully",
                    doctor = new DoctorResponseDTO
                    {
                        doctorID = newDoctor.id,
                        UserID = newUser.id,
                        Email = newUser.Email,
                        FirstName = newDoctor.FirstName,
                        LastName = newDoctor.LastName,
                        Address = newDoctor .Address,
                        Phone = newDoctor.Phone,
                        EPhone = newDoctor.EPhone
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

        [HttpPatch("UpdateDoctor{doctorID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorResponseDTO>> UpdateDoctor(int doctorID, [FromBody] DoctorDTO doctor)
        {
            if (doctorID <= 0)
                return BadRequest($"The ID: {doctorID} is not valid.");

            var doc = await _context.doctors
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == doctorID);

            if (doc == null)
                return NotFound($"No doctor found with ID {doctorID}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ تحقق من البريد الإلكتروني الجديد فقط إذا تم إدخاله وكان مختلفًا
                if (!string.IsNullOrWhiteSpace(doctor.Email) &&
                    doc.User != null &&
                    doc.User.Email != doctor.Email)
                {
                    bool emailExists = await _context.users.AnyAsync(u => u.Email == doctor.Email && u.id != doc.User.id);
                    if (emailExists)
                        return Conflict("Email already exists for another user.");

                    doc.User.Email = doctor.Email;
                }

                // ✅ تحديث الحقول فقط إذا أرسل المستخدم قيمًا جديدة
                if (!string.IsNullOrWhiteSpace(doctor.FirstName))
                    doc.FirstName = doctor.FirstName;

                if (!string.IsNullOrWhiteSpace(doctor.LastName))
                    doc.LastName = doctor.LastName;

                if (!string.IsNullOrWhiteSpace(doctor.Address))
                    doc.Address = doctor.Address;

                if (!string.IsNullOrWhiteSpace(doctor.Phone))
                    doc.Phone = doctor.Phone;

                if (!string.IsNullOrWhiteSpace(doctor.EPhone))
                    doc.EPhone = doctor.EPhone;

                // ✅ تحديث كلمة المرور فقط إذا تم إرسالها
                if (!string.IsNullOrWhiteSpace(doctor.Password) && doc.User != null)
                    doc.User.Password = BCrypt.Net.BCrypt.HashPassword(doctor.Password);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Doctor updated successfully",
                    doctor = new DoctorResponseDTO
                    {
                         doctorID = doc.id,
                        UserID = doc.UserId,
                        Email = doc.User?.Email!,
                        FirstName = doc.FirstName,
                        LastName = doc.LastName,
                        Address = doc.Address,
                        Phone = doc.Phone,
                        EPhone = doc.EPhone
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Update failed: {ex.Message}");
            }
        }


        [HttpGet("AllDoctor")]
        public async Task<ActionResult<IEnumerable<clsDoctor>>> AllDoctor()
        {
            var doctors = await _context.doctors
                .Include(s => s.User)
                .ToListAsync();

            if (doctors.Count == 0)
                return NotFound("There are no doctors in the system.");

            return Ok(
                doctors.Select(s => new DoctorResponseDTO
                {
                    doctorID = s.id,
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
        [Authorize(Roles = "admin")]
        [HttpGet("GetDoctorByID{DoctorID}", Name = "GetDoctorByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResponseDTO>> GetDoctorByID(int DoctorID)
        {
            if (DoctorID <= 0)
                return BadRequest("Please Insert Valied ID Between {1 ,,, 1000}");

            var doc = await _context.doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.id == DoctorID);
            if (doc == null)
            {
                return NotFound($"There is no doctor with ID {DoctorID}");
            }


            return Ok(new DoctorResponseDTO
            {
                Address = doc.Address,
                Email = doc.User.Email,
                EPhone = doc.EPhone,
                Phone = doc.Phone,
                FirstName = doc.FirstName,
                LastName = doc.LastName,
                doctorID = doc.id,
                UserID = doc.UserId
            });
        }

    
        [HttpDelete("DeleteDoctor{DoctorID}")]
        public async Task<IActionResult> DeleteDoctor(int DoctorID)
        {
            if (DoctorID <= 0)
                return BadRequest($"The ID: {DoctorID} is not valid.");

            var doctor = await _context.doctors
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == DoctorID);

            if (doctor == null)
                return NotFound($"There is no Doctor with ID {DoctorID}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (doctor.User != null)
                    _context.users.Remove(doctor.User);

                _context.doctors.Remove(doctor);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Doctor and related User deleted successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting doctor.");
            }
        }

    }
}
