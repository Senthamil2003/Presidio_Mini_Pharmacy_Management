﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.RequestDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewController : ControllerBase
    {
        private readonly IViewService _viewService;
        private readonly ILogger<ViewController> _logger;

        public ViewController(IViewService viewService, ILogger<ViewController> logger)
        {
            _viewService = viewService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all the available products in stock.
        /// </summary>
        /// <returns>An array of stock response details for all available products.</returns>
        [HttpGet("ViewAllItems")]
        [ProducesResponseType(typeof(StockResponseDTO[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StockResponseDTO[]>> GetAllProduct()
        {
            try
            {
                _logger.LogInformation("Received a request to view all available products.");

                var result = await _viewService.ShowAllProduct();
                _logger.LogInformation("Available products retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving available products: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the orders for the current user.
        /// </summary>
        /// <returns>A list of order details for the current user.</returns>
        [HttpGet("ViewMyOrders")]
        [ProducesResponseType(typeof(List<MyOrderDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MyOrderDTO>>> ViewMyOrders()
        {
            try
            {
                _logger.LogInformation("Received a request to view user's orders.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _viewService.GetAllOrders(userid);
                _logger.LogInformation("User's orders retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user's orders: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the medications for the current user.
        /// </summary>
        /// <returns>A list of medication details for the current user.</returns>
        [HttpGet("ViewMyMedications")]
        [ProducesResponseType(typeof(List<AddMedicationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<AddMedicationDTO>>> ViewMyMedications()
        {
            try
            {
                _logger.LogInformation("Received a request to view user's medications.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _viewService.ViewMyMedications(userid);
                _logger.LogInformation("User's medications retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user's medications: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the cart items for the current user.
        /// </summary>
        /// <returns>A list of cart item details for the current user.</returns>
        [HttpGet("ViewMyCart")]
        [ProducesResponseType(typeof(List<MyCartDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<MyCartDTO>>> ViewMyCart()
        {
            try
            {
                _logger.LogInformation("Received a request to view user's cart.");

                var userstring = User.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
                var userid = Convert.ToInt32(userstring);
                var result = await _viewService.ViewMyCart(userid);
                _logger.LogInformation("User's cart retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving user's cart: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}