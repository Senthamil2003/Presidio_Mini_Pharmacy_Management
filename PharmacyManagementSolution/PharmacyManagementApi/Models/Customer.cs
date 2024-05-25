using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string Name { get; set; }
        [Required]
        public string Email {  get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Role { get; set; }
        public bool IsSubcribed { get; set; }=false;
        public ICollection<Order> Orders { get; set; }
        public ICollection<Cart> Carts { get; set; }


    }
}
