using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.DTOs.Response;
using Lpgin2.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDBContext _context;

        public AdminController(AppDBContext context)
        {
            _context = context;
        }
        [HttpPost("AddNewAdmin", Name = "AddNewAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddNewAdmin([FromBody] AdminDTO admin)
        {
            if (admin == null)
                return BadRequest("Invalid Admin data.");

            //IDbContextTransaction  ال Var يكون من نوع
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // التحقق من وجود البريد الإلكتروني قبل الإدراج
                if (await _context.users.AnyAsync(u => u.Email == admin.Email))
                    return Conflict("Email already exists. Please use another email.");

                var newUser = new clsUser
                {
                    Email = admin.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(admin.Password),
                    role = Enums.enUserRoles.admin
                };
                
                _context.users.Add(newUser);
                await _context.SaveChangesAsync();

                var newAdmin = new clsAdmin
                {
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Address = admin.Address,
                    Phone = admin.Phone,
                    EPhone = admin.EPhone,
                    UserId = newUser.id
                };

                _context.admins.Add(newAdmin);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "User registered successfully",
                    admin = new AdminResponseDTO
                    {
                        adminID = newAdmin.id,
                        UserID = newUser.id,
                        Email = newUser.Email,
                        FirstName = newAdmin.FirstName,
                        LastName = newAdmin.LastName,
                        Address = newAdmin.Address,
                        Phone = newAdmin.Phone,
                        EPhone = newAdmin.EPhone
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

        [HttpPatch("UpdateAdmin{adminID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AdminResponseDTO>> UpdateAdmin(int adminID, [FromBody] AdminDTO admin)
        {
            if (adminID <= 0)
                return BadRequest($"The ID: {adminID} is not valid.");

            var adm = await _context.admins
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == adminID);

            if (adm == null)
                return NotFound($"No admin found with ID {adminID}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // ✅ تحقق من البريد الإلكتروني الجديد فقط إذا تم إدخاله وكان مختلفًا
                if (!string.IsNullOrWhiteSpace(admin.Email) &&
                    adm.User != null &&
                    adm.User.Email != admin.Email)
                {
                    bool emailExists = await _context.users.AnyAsync(u => u.Email == admin.Email && u.id != adm.User.id);
                    if (emailExists)
                        return Conflict("Email already exists for another user.");

                    adm.User.Email = admin.Email;
                }

                // ✅ تحديث الحقول فقط إذا أرسل المستخدم قيمًا جديدة
                if (!string.IsNullOrWhiteSpace(admin.FirstName))
                    adm.FirstName = admin.FirstName;

                if (!string.IsNullOrWhiteSpace(admin.LastName))
                    adm.LastName = admin.LastName;

                if (!string.IsNullOrWhiteSpace(admin.Address))
                    adm.Address = admin.Address;

                if (!string.IsNullOrWhiteSpace(admin.Phone))
                    adm.Phone = admin.Phone;

                if (!string.IsNullOrWhiteSpace(admin.EPhone))
                    adm.EPhone = admin.EPhone;

                // ✅ تحديث كلمة المرور فقط إذا تم إرسالها
                if (!string.IsNullOrWhiteSpace(admin.Password) && adm.User != null)
                    adm.User.Password = BCrypt.Net.BCrypt.HashPassword(admin.Password);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    message = "Admin updated successfully",
                    admin = new AdminResponseDTO
                    {
                        adminID = adm.id,
                        UserID = adm.UserId,
                        Email = adm.User?.Email!,
                        FirstName = adm.FirstName,
                        LastName = adm.LastName,
                        Address = adm.Address,
                        Phone = adm.Phone,
                        EPhone = adm.EPhone
                    }
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, $"Update failed: {ex.Message}");
            }
        }



        [HttpGet("AllAdmins")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<IEnumerable<clsAdmin>>> AllAdmins()
        {
            var admins = await _context.admins
                .Include(s => s.User)
                .ToListAsync();

            if (admins.Count == 0)
                return NotFound("There are  no admins in the system.");

            return Ok(
                admins.Select(s => new AdminResponseDTO
                {
                    adminID = s.id,
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


        [HttpGet("GetAdminByID{AdminId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdminResponseDTO>> GetAdminByID(int AdminId)
        {
            if (AdminId <= 0)
                return BadRequest("Please Insert Valied ID Between {1 ,,, 1000}");

            var doc = await _context.admins
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.id == AdminId);
            if (doc == null)
            {
                return NotFound($"There is no admin with ID {AdminId}");
            }


            return Ok(new AdminResponseDTO
            {
                Address = doc.Address,
                Email = doc.User.Email,
                EPhone = doc.EPhone,
                Phone = doc.Phone,
                FirstName = doc.FirstName,
                LastName = doc.LastName,
                adminID = doc.id,
                UserID = doc.UserId
            });
        }


        [HttpDelete("DeleteAdmin{AdminId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAdmin(int AdminId)
        {
            if (AdminId <= 0)
                return BadRequest($"The ID: {AdminId} is not valid.");

            var admin = await _context.admins
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.id == AdminId);

            if (admin == null)
                return NotFound($"There is no admin with ID {AdminId}");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (admin.User != null)
                    _context.users.Remove(admin.User);

                _context.admins.Remove(admin);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok("Admin and related User deleted successfully.");
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting admin.");
            }
        }
    }
}
