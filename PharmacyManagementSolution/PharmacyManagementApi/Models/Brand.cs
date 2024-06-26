using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models
{
    public class Brand { 
        [Key]
        public int BrandId { get; set; }    
        public string BrandName { get; set; }
        public byte[]? Image { get; set; }



    }
}
