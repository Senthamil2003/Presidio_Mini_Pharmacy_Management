using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; } 
        public int CustomerId { get; set; }
        public int StockId { get; set; }
        public int Cost { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }


    }
}
