using BCrypt.Net;
using Lpgin2.Data;
using Lpgin2.DTOs.Auth;
using Lpgin2.DTOs.Request;
using Lpgin2.Models.Entities;
using Lpgin2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;

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
        [EnableRateLimiting("AuthLimiter")]
        public async Task<IActionResult> login(LoginRequest credentials)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == credentials.Email);

            if (user == null|| !BCrypt.Net.BCrypt.Verify(credentials.Password , user.Password))
                return Unauthorized("Invalid username or password");

            var token = _Tokenservice.GenerateToken(user);

            user.RefreshTokenHash =
                BCrypt.Net.BCrypt.HashPassword(token.RefreshToken);

            user.RefreshTokenExpiresAt =
                DateTime.UtcNow.AddDays(7);

            user.RefreshTokenRevokedAt = null;

            await _context.SaveChangesAsync();

            return Ok(new { AccessToken = token.AccessToken,RefreshToken =token.RefreshToken });
        }

        [HttpPost("Refresh")]
        [EnableRateLimiting("AuthLimiter")]
        public async Task< IActionResult> Refresh([FromBody] RefreshRequest request)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return Unauthorized("Invalid refresh request");


            if (user.RefreshTokenRevokedAt != null)
                return Unauthorized ("Refresh token is revoked");

            if (user.RefreshTokenExpiresAt == null ||
                user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expired");

            bool refreshValid =
              BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);

            if (!refreshValid)
                return Unauthorized("Invalid refresh token");

            TokenResponse token = _Tokenservice.Refresh(request,user);
            return Ok(new { AccessToken = token.AccessToken, RefreshToken = token.RefreshToken });



        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var user =await _context.users.FirstOrDefaultAsync(s => s.Email == request.Email);

            if (user == null)
                return Ok(); 

            bool refreshValid = BCrypt.Net.BCrypt.Verify(request.RefreshToken, user.RefreshTokenHash);
            if (!refreshValid)
                return Ok();

            user.RefreshTokenRevokedAt = DateTime.UtcNow;
            return Ok("Logged out successfully");
        }

    }
}
