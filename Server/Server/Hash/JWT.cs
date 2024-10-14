using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Server.Controllers
{
    internal class JWT
    {
        private readonly byte[] Key;

        public JWT() {}

        public string GenerateJwtToken(string userId, string Key, int Hours, string userRoleId = null)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.Now.AddHours(Hours);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (!string.IsNullOrEmpty(userRoleId)) { claims.Add(new Claim("Role", userRoleId)); }

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token).ToString();
        }

        public string GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);

            return userIdClaim?.Value;
        }

        public bool ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwtToken = handler.ReadJwtToken(token);
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub);

                if (userIdClaim == null)
                {
                    return false;
                }

                var expiration = jwtToken.ValidTo;
                return expiration > DateTime.UtcNow;
            }
            catch
            {
                return false;
            }
        }
    }
}
