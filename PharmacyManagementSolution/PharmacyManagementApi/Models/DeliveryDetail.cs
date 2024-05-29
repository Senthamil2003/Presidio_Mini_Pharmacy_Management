using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class DeliveryDetail
    {
        [Key]
        public int DeliveryId { get; set; } 
        public int CustomerId { get; set; }
        public int OrderDetailId { get; set; }
        public int MedicineId { get; set; } 
        public DateTime DeliveryDate { get; set; } = DateTime.Now;
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }

        [ForeignKey("OrderDetailId")]
        public OrderDetail OrderDetail { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

    }
}
