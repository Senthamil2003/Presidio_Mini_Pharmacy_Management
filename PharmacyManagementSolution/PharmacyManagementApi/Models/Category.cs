using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public byte[]? Image { get; set; }

    }
}
