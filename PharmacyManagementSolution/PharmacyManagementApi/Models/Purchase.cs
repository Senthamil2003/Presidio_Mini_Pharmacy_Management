using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; } 
        public DateTime PurchaseDate { get; set; }=DateTime.Now;
        public int VendorId { get; set; }
        public double TotalAmount { get; set; }
        [ForeignKey("VendorId")]
        public Vendor Vendor { get; set; }
        public ICollection<PurchaseDetail> PurchaseDetails { get; set; }

    }
}
