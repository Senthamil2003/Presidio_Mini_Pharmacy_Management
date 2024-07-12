using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models.DTO.ErrorDTO;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyCors")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportService reportService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the purchase report for a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>A list of purchase report details for the specified date range.</returns>
        [HttpGet("PurchaseReport")]
        [ProducesResponseType(typeof(List<PurchaseReportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<PurchaseReportDTO>>> PurchaseReport(DateTime startDate, DateTime endDate)
        {
            try
            {
               

                var result = await _reportService.GetPurchasesReport(startDate, endDate);
                _logger.LogInformation("Purchase report retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving purchase report: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }

        /// <summary>
        /// Retrieves the order report for a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>A list of order report details for the specified date range.</returns>
        [HttpGet("OrderReport")]
        [ProducesResponseType(typeof(List<OrderReportDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<OrderReportDTO>>> OrderReport(DateTime startDate, DateTime endDate)     
        {
            try
            {
               

                var result = await _reportService.GetOrderReport(startDate,endDate);
                _logger.LogInformation("Order report retrieved successfully.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving order report: {ex.Message}");
                return NotFound(new ErrorModel(404, ex.Message));
            }
        }
    }
}