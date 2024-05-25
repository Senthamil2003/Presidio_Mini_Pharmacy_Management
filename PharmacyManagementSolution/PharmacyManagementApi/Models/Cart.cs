using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; } 
        public int CustomerId { get; set; }
        public int MedicineId { get; set; }
        public int Quantity { get; set; }   
        public double Cost { get; set; }
        public double TotalCost { get; set; }
        public DateTime Date { get; set; }= DateTime.Now;

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }

        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }


    }
}
