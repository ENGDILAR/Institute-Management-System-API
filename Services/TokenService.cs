using Lpgin2.DTOs.Auth;
using Lpgin2.DTOs.Response;
using Lpgin2.Helpers;
using Lpgin2.Models;
using Lpgin2.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Lpgin2.Services
{
    public class TokenService
    {
        private readonly JwtOptions _opts;
   

        public TokenService(IOptions<JwtOptions> jwtopts)
        {
            _opts = jwtopts.Value;
         
        }

        public TokenResponse GenerateToken(clsUser user)
        {    

            var claims = new[]
            {
                 new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                 new Claim(ClaimTypes.Role, user.role.ToString()),
                 new Claim(ClaimTypes.Email, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
         
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
            var AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var RefreshToken = TokenService.GenerateRefreshToken();
            return (new TokenResponse { AccessToken = AccessToken,RefreshToken=RefreshToken });
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public TokenResponse Refresh(RefreshRequest request, clsUser user)
        {

          

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.role.ToString())
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_opts.SigningKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                 issuer: _opts.Issuer,
                 audience: _opts.Audience,
                 claims: claims,
                 expires: DateTime.UtcNow.AddMinutes(_opts.LifetimeInMinutes),
                 signingCredentials: creds
             );

            var accessToken =
                new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = TokenService.GenerateRefreshToken();

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

       
    }

}

