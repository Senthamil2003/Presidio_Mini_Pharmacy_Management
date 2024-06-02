using PharmacyManagementApi.Models.DTO.ResponseDTO;

namespace PharmacyManagementApi.Interface
{
    public interface IReportService
    {
        public  Task<IEnumerable<PurchaseReportDTO>> GetPurchasesReport(DateTime startDate, DateTime endDate);
        public  Task<IEnumerable<OrderReportDTO>> GetOrderReport(DateTime startDate, DateTime endDate);
    }
}
