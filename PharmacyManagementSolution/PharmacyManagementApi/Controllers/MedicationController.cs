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
    public class MedicationController : ControllerBase
    {
        private readonly IMedicationService _medicationService;

        public MedicationController(IMedicationService medicationService) {

            _medicationService=medicationService;
        
        }

        [HttpPost("AddMedication")]
        [ProducesResponseType(typeof(SuccessMedicationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessMedicationDTO>> AddToCart(AddMedicationDTO add)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                add.CustomerId = userid;
                var result = await _medicationService.AddMedication(add);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPut("UpdateMedication")]
        [ProducesResponseType(typeof(SuccessMedicationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessMedicationDTO>> UpdateMedication(UpdateMedication update)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                update.CustomerId = userid;
                var result = await _medicationService.UpdateMedication(update);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPost("BuyMedication")]
        [ProducesResponseType(typeof(SuccessCheckoutDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCheckoutDTO>> BuyMedication(int medicationId )
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);

                var result = await _medicationService.BuyMedication(userid,medicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }


    }
}
