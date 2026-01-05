using BCrypt.Net;
using Lpgin2.Data;
using Lpgin2.DTOs.Request;
using Lpgin2.Models.Entities;
using Lpgin2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Lpgin2.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDBContext _context;
        private readonly TokenService _Tokenservice;
        public AuthController(AppDBContext context,TokenService tokenService)
        {
           _context = context;
           _Tokenservice = tokenService ;
        }



        [HttpPost("Login")]
        public async Task<IActionResult> login(UserLoginDto credentials)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == credentials.Email);

            if (user == null|| !BCrypt.Net.BCrypt.Verify(credentials.Password , user.Password))
                return Unauthorized("Invalid username or password");
            var token = _Tokenservice.GenerateToken(user);
            return Ok(new { Token = token });
        }
    }
}
