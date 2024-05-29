using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Services;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : ControllerBase
    {
        private readonly IViewService _viewService;

        public ViewController(IViewService viewService) {
            _viewService=viewService;
        }
        [HttpGet("ViewAllItems")]
        [ProducesResponseType(typeof(StockResponseDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StockResponseDTO[]>> GetAllProduct()
        {
            try
            {
                var result = await _viewService.ShowAllProduct();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("ViewMyOrders")]
        [ProducesResponseType(typeof(List<MyOrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MyOrderDTO>>> AllOrder()
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _viewService.GetAllOrders(userid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("ViewMyMedications")]
        [ProducesResponseType(typeof(List<AddMedicationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<AddMedicationDTO>>> AllMedication()
        {
            try
            {
                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _viewService.ViewMyMedications(userid);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new ErrorModel(501, ex.Message));
            }
        }


    }
}
