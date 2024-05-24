using Microsoft.IdentityModel.Tokens;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PharmacyManagementApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _secretKey;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _secretKey = configuration.GetSection("TokenKey").GetSection("JWT").Value.ToString();
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        }
        public async Task<string> GenerateToken(Customer employee)
        {
            string token = string.Empty;
            var claims = new List<Claim>(){
                new Claim("Id", employee.CustomerId.ToString()),
                new Claim(ClaimTypes.Role,employee.Role),

            };
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var myToken = new JwtSecurityToken(null, null, claims, expires: DateTime.Now.AddDays(2), signingCredentials: credentials);
            token = new JwtSecurityTokenHandler().WriteToken(myToken);
            return token;

        }
    }
}

