using Lpgin2.Models.Entities;
using Lpgin2.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Lpgin2.Helpers;

namespace Lpgin2.Services
{
    public class TokenService
    {
        private readonly JwtOptions _opts;
   

        public TokenService(IOptions<JwtOptions> jwtopts)
        {
            _opts = jwtopts.Value;
         
        }

        public string GenerateToken(clsUser user)
        {    

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
            var key = Encoding.UTF8.GetBytes(_opts.SigningKey);
            var signingKey = new SymmetricSecurityKey(key);
            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);


          
     
            var token = new JwtSecurityToken(
                issuer: _opts.Issuer,
                audience: _opts.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_opts.LifetimeInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
