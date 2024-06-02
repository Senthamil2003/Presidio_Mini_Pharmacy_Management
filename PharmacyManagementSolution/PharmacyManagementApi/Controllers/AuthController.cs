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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="userLoginDTO">The user login credentials.</param>
        /// <returns>A success response with the JWT token on successful authentication.</returns>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(SuccessLoginDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SuccessLoginDTO>> Login(LoginDTO userLoginDTO)
        {
            try
            {
                _logger.LogInformation("Received a user login request.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the login request.");
                    return BadRequest(ModelState);
                }

                var result = await _authService.Login(userLoginDTO);
                _logger.LogInformation("User authenticated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during user authentication: {ex.Message}");
                return Unauthorized(new ErrorModel(401, ex.Message));
            }
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDTO">The user registration details.</param>
        /// <returns>A success response on successful registration.</returns>
        [HttpPost("Register")]
        [ProducesResponseType(typeof(SuccessRegisterDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessRegisterDTO>> Register(RegisterDTO userDTO)
        {
            try
            {
                _logger.LogInformation("Received a user registration request.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the registration request.");
                    return BadRequest(ModelState);
                }

                SuccessRegisterDTO result = await _authService.Register(userDTO);
                _logger.LogInformation("User registered successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred during user registration: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}