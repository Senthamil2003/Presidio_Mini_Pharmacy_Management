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
        public Double Cost { get; set; }
    

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }

      
        public ICollection<DeliveryDetail> DeliveryDetails { get; set;}
       
    }
}
