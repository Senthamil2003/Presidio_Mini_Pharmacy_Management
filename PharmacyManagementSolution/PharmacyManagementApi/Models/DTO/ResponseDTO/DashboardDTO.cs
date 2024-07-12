namespace PharmacyManagementApi.Models.DTO.ResponseDTO
{
    public class DashboardDTO
    {
        public int MedicineCount { get; set; }
        public int CustomerCount { get; set; }
        public double PurchaseAmount { get; set; }
        public double OrdersAmount { get; set; }    
    }
}
