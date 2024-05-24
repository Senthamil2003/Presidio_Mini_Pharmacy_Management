namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class PurchaseDTO
    {
        public string VendorName { get; set; }
        public string MedicineName { get; set; }
        public string MedicineCategory { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string StorageRequirement { get; set; }
        public string DosageForm { get; set; }
    }
}
