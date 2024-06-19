using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Services;

namespace PharmacyManagementApi.Controllers
{
 
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCors")]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService purchaseService, ILogger<AdminController> logger)
        {
            _adminService = purchaseService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new vendor to the system.
        /// </summary>
        /// <param name="vendor">The vendor details.</param>
        /// <returns>A success response on successful addition of the vendor.</returns>
        [HttpPost("AddVendor")]
        [ProducesResponseType(typeof(SuccessDeliveryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessDeliveryDTO>> AddVendor(VendorDTO vendor)
        {
            try
            {
                _logger.LogInformation("Received a request to add a new vendor.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the vendor request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.AddVendor(vendor);
                _logger.LogInformation("Vendor added successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while adding a vendor: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Purchases medicine from a vendor.
        /// </summary>
        /// <param name="purchaseDTO">The purchase details.</param>
        /// <returns>A success response on successful purchase.</returns>
        [HttpPost("Purchase")]
        [ProducesResponseType(typeof(SuccessPurchaseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessPurchaseDTO>> PurchaseElement(PurchaseDTO purchaseDTO)
        {
            try
            {
                _logger.LogInformation("Received a request to purchase medicine.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the purchase request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.PurchaseMedicine(purchaseDTO);
                _logger.LogInformation("Medicine purchased successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while purchasing medicine: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves all orders in the system.
        /// </summary>
        /// <returns>An array of order details.</returns>
        [HttpGet("GetAllOrders")]
        [ProducesResponseType(typeof(OrderDetailDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDetailDTO[]>> GetAllOrders()
        {
            try
            {
                _logger.LogInformation("Received a request to retrieve all orders.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the get all orders request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.GetAllOrder();
                _logger.LogInformation("Orders retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving orders: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }

        /// <summary>
        /// Delivers an order.
        /// </summary>
        /// <param name="orderDetailId">The ID of the order to be delivered.</param>
        /// <returns>A success response on successful delivery of the order.</returns>
        [HttpPost("DeliverOrder")]
        [ProducesResponseType(typeof(SuccessDeliveryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SuccessDeliveryDTO>> DeliverOrder(int orderDetailId)
        {
            try
            {
                _logger.LogInformation($"Received a request to deliver order with ID: {orderDetailId}.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the deliver order request.");
                    return BadRequest(ModelState);
                }

                var result = await _adminService.DeliverOrder(orderDetailId);
                _logger.LogInformation("Order delivered successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while delivering order: {ex.Message}");
                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }
}