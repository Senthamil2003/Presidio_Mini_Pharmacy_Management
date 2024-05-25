using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public double PaidAmount { get; set; }
        public double TotalAmount { get; set; }
        public float Discount {  get; set; }
        public double ShipmentCost { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; }

    }
}
