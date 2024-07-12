using PharmacyManagementApi.Models;
using System.Security.Claims;

namespace PharmacyManagementApi.Interface
{
    public interface ITokenService
    {
        public (bool isValid, ClaimsPrincipal? claimsPrincipal) ValidateToken(string token);
        public Task<string> GenerateToken(Customer login);
    }
}
