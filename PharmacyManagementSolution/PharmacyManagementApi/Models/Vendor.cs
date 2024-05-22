using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models
{
    public class Vendor
    {
        [Key]
        public int VendorId { get; set; }
        public string VendorName { get; set;}
        public string Address { get; set;}
        public string Phone { get; set;}

    }
}
