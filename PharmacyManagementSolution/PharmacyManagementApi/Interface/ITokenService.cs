using PharmacyManagementApi.Models;

namespace PharmacyManagementApi.Interface
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(Customer login);
    }
}
