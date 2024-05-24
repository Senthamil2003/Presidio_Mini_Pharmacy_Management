using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IAuthService
    {
        public Task<SuccessLoginDTO> Login(LoginDTO loginDTO);
        public Task<SuccessRegisterDTO> Register(RegisterDTO employeeDTO);

    }
}
