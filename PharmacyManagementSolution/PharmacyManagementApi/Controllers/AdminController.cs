using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;
using PharmacyManagementApi.Services;

namespace PharmacyManagementApi.Controllers
{
 
    //[Authorize(Roles = "Admin")]
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
        [HttpGet("DeliverOrder")]
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
        [HttpGet("GetAllMedicine")]
        [ProducesResponseType(typeof(List<MedicineDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<MedicineDTO>>> GetAllMedicine()
        {
            try
            {

                var result = await _adminService.GetAllMedicine();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetCategory")]
        [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Category>> GetCategory(int CategoryId)
        {
            try
            {

                var result = await _adminService.GetCategory(CategoryId);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPut("UpdateMedicine")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UpdateMedicine([FromForm] UpdateMedicineDTO update)
        {
            try
            {

                var result = await _adminService.UpdateMedicine(update);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetMedicine")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicineDetailDTO>> GetMedicine(int  MedicineId)
        {
            try
            {

                var result = await _adminService.GetMedicine(MedicineId);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpPost("AddMedicine")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicineDetailDTO>> AddMedicine([FromForm] AddMedicineDTO addMedicine)
        {
            try
            {

                var result = await _adminService.AddMedicine(addMedicine);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetAllCategory")]
        [ProducesResponseType(typeof(List<CategoryDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<CategoryDTO>>> GetAllCategory()
        {
            try
            {

                var result = await _adminService.GetAllCategory();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
        [HttpGet("GetAllBrand")]
        [ProducesResponseType(typeof(List<BrandDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<List<BrandDTO>>> GetAllBrand()
        {
            try
            {

                var result = await _adminService.GetAllBrand();

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(new ErrorModel(501, ex.Message));
            }
        }
    }

}
