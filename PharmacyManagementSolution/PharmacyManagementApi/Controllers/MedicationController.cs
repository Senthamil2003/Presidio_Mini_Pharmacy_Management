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
        private readonly ILogger<MedicationController> _logger;

        public MedicationController(IMedicationService medicationService, ILogger<MedicationController> logger)
        {
            _medicationService = medicationService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new medication for the current user.
        /// </summary>
        /// <param name="add">The medication details.</param>
        /// <returns>A success response on successful addition of the medication.</returns>
        [HttpPost("CreateMedication")]
        [ProducesResponseType(typeof(SuccessMedicationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessMedicationDTO>> AddMedication(AddMedicationDTO add)
        {
            try
            {
                _logger.LogInformation("Received a request to add a new medication.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                add.CustomerId = userid;
                var result = await _medicationService.AddMedication(add);
                _logger.LogInformation("Medication added successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding a medication: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Updates an existing medication for the current user.
        /// </summary>
        /// <param name="update">The updated medication details.</param>
        /// <returns>A success response on successful update of the medication.</returns>
        [HttpPut("UpdateMedication")]
        [ProducesResponseType(typeof(SuccessMedicationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessMedicationDTO>> UpdateMedication(UpdateMedication update)
        {
            try
            {
                _logger.LogInformation("Received a request to update a medication.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                update.CustomerId = userid;
                var result = await _medicationService.UpdateMedication(update);
                _logger.LogInformation("Medication updated successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating a medication: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Purchases a medication for the current user.
        /// </summary>
        /// <param name="medicationId">The ID of the medication to be purchased.</param>
        /// <returns>A success response with the order details.</returns>
        [HttpPost("BuyMedication")]
        [ProducesResponseType(typeof(SuccessCheckoutDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessCheckoutDTO>> BuyMedication(int medicationId)
        {
            try
            {
                _logger.LogInformation($"Received a request to purchase medication with ID: {medicationId}.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _medicationService.BuyMedication(userid, medicationId);
                _logger.LogInformation("Medication purchased successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while purchasing a medication: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPut("AddMedicationItem")]
        [ProducesResponseType(typeof(SuccessMedicationDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessMedicationDTO>> AddMedicationItem(AddMedicationItemDTO updateMedication)
        {
            try
            {

                var result = await _medicationService.AddMedicationItem(updateMedication);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpDelete("RemoveMedication")]
        [ProducesResponseType(typeof(SuccessRemoveDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessRemoveDTO>> DeleteMedication(int medicationId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _medicationService.RemoveMedication(userid,medicationId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpDelete("RemoveMedicationItem")]
        [ProducesResponseType(typeof(SuccessRemoveDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessRemoveDTO>> DeleteMedicationItem(int medicationId,int medicationItemId)
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _medicationService.RemoveMedicationItem( medicationId,userid, medicationItemId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }

}