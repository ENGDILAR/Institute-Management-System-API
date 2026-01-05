using Lpgin2.Data;
using Lpgin2.Models.Entities;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Lpgin2.Services
{
    public class OldJWTService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDBContext _context;

        public OldJWTService(IConfiguration configuration, AppDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(clsUser user)
        {

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            // the ! mean that you tell the compiler that the value is not null surely
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SigningKey"]!);


            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                 new Claim(ClaimTypes.Role, user.role.ToString()),
                 new Claim("UserId", user.id.ToString()),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            // EX
            //            {
            //                "sub": "dilar@gmail.com",
            //                "role": "Admin",
            //                "UserId": "7",
            //                "jti": "c8a5b87b-2a0d-4b2b-bc25-7e5dfe0cd07e",
            //                "iss": "yourApp",
            //                "aud": "yourUsers",
            //                "exp": 1730000000 Timer
            //            }

            // 🔐 توقيع التوكن
            var signingKey = new SymmetricSecurityKey(key);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


            // ⏳ إعداد مدة صلاحية التوكن
            var expiryMinutes = int.Parse(_configuration["Jwt:LifetimeInMinutes"] ?? "30");
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
