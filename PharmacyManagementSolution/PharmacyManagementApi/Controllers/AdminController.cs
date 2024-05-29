using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Services;

namespace PharmacyManagementApi.Controllers
{
   
    //[Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
   
    public class AdminController : ControllerBase
    {
        public readonly IAdminService _purchaseService;
       public  AdminController(IAdminService purchaseService) {
            _purchaseService = purchaseService;
        }
        [HttpPost("Purchase")]
        [ProducesResponseType(typeof(SuccessPurchaseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessPurchaseDTO>> PurchaseElement(PurchaseDTO purchaseDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _purchaseService.PurchaseMedicine(purchaseDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetAllOrders")]
        [ProducesResponseType(typeof(OrderDetailDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDetailDTO[]>> GetAllOrders()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _purchaseService.GetAllOrder();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        [HttpPost("DeliverOrder")]
        [ProducesResponseType(typeof(SuccessDeliveryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessDeliveryDTO>> DeliverOrder(int orderDetailId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _purchaseService.DeliverOrder(orderDetailId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}
