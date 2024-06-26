using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }
        public int PurchaseDetailId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
      
        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

        [ForeignKey("PurchaseDetailId")]
        public PurchaseDetail PurchaseDetail { get; set; }

    }
}
