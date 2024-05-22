using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int MedicineId { get; set; }
        public int PurchaseDetailId { get; set; }
        public int Cost { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

        [ForeignKey("PurchaseDetailId")]
        public PurchaseDetail PurchaseDetail { get; set; }
       
    }
}
