using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class PurchaseDetail
    {
        [Key]
        public int PurchaseDetailId { get; set; }
        public int PurchaseId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string StorageRequirement { get; set; }
        public string DosageForm { get; set; }

        [ForeignKey("PurchaseId")]
        public Purchase Purchase { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }
    }
}
