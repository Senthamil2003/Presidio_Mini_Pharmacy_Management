using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) {
            _authService=authService;

        }
        [HttpPost("Login")]
        [ProducesResponseType(typeof(SuccessLoginDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SuccessLoginDTO>> Login(LoginDTO userLoginDTO)
        {
            try
            {
                var result = await _authService.Login(userLoginDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
        }
        [HttpPost("Register")]
        [ProducesResponseType(typeof(SuccessRegisterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessRegisterDTO>> Register(RegisterDTO userDTO)
        {
            try
            {
                SuccessRegisterDTO result = await _authService.Register(userDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

    }
}
