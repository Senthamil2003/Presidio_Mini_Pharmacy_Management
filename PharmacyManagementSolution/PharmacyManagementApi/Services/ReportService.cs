using Microsoft.EntityFrameworkCore;
using PharmacyManagementApi.CustomException;
using PharmacyManagementApi.Interface;
using PharmacyManagementApi.Models;
using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Services
{
    public class ReportService : IReportService
    {
        private readonly IRepository<int, PurchaseDetail> _purchaseDetailRepo;
        private readonly IRepository<int, OrderDetail> _orderDetailRepo;
        private readonly ILogger<ReportService> _logger;

        public ReportService(IRepository<int, PurchaseDetail> purchaseDetail, IRepository<int, OrderDetail> orderDetail, ILogger<ReportService> logger)
        {
            _purchaseDetailRepo = purchaseDetail;
            _orderDetailRepo = orderDetail;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the purchase report for a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>A collection of purchase report details for the specified date range.</returns>
        public async Task<IEnumerable<PurchaseReportDTO>> GetPurchasesReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Retrieving purchase report from {startDate} to {endDate}.");

                var report = (await _purchaseDetailRepo.Get())
                    .Where(pd => pd.Purchase.PurchaseDate >= startDate && pd.Purchase.PurchaseDate <= endDate)
                    .GroupBy(pd => new
                    {
                        pd.Purchase.PurchaseDate,
                        pd.MedicineId,
                        pd.Medicine.MedicineName
                    })
                    .Select(g => new PurchaseReportDTO
                    {
                        PurchaseDate = g.Key.PurchaseDate,
                        MedicineId = g.Key.MedicineId,
                        MedicineName = g.Key.MedicineName,
                        TotalAmount = g.Sum(pd => pd.TotalSum),
                        TotalQuantity = g.Sum(pd => pd.Quantity)
                    })
                    .OrderByDescending(r => r.TotalQuantity)
                    .ToList();

                if (report.Count == 0)
                {
                    _logger.LogWarning("The purchase report is empty.");
                    throw new NoPurchaseDetailFoundException("The purchase report is empty");
                }

                _logger.LogInformation("Purchase report retrieved successfully.");
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the purchase report: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves the order report for a given date range.
        /// </summary>
        /// <param name="startDate">The start date of the date range.</param>
        /// <param name="endDate">The end date of the date range.</param>
        /// <returns>A collection of order report details for the specified date range.</returns>
        public async Task<IEnumerable<OrderReportDTO>> GetOrderReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation($"Retrieving order report from {startDate} to {endDate}.");

                var report = (await _orderDetailRepo.Get())
                    .Where(od => od.Order.OrderDate >= startDate && od.Order.OrderDate <= endDate)
                    .GroupBy(od => new
                    {
                        od.Order.OrderDate,
                        od.MedicineId,
                        od.Medicine.MedicineName,
                    })
                    .Select(g => new OrderReportDTO
                    {
                        OrdereDate = g.Key.OrderDate,
                        MedicineId = g.Key.MedicineId,
                        MedicineName = g.Key.MedicineName,
                        TotalAmount = g.Sum(od => od.Cost),
                        TotalQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(r => r.TotalQuantity)
                    .ToList();

                if (report.Count == 0)
                {
                    _logger.LogWarning("The order report is empty.");
                    throw new NoOrderDetailFoundException("The Order report is empty");
                }

                _logger.LogInformation("Order report retrieved successfully.");
                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving the order report: {ex.Message}");
                throw;
            }
        }
    }
}