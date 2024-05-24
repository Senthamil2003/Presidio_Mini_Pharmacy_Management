using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class UserCredential
    {
        [Key]
        public string Email { get; set; }
        public int  UserId { get; set; }
        public byte[] Password { get; set; }
        public byte[] HasedPassword { get; set; }
        public int BankBalance { get; set; } = 0;
        public string AccountStatus { get; set; } = "Disable";
        [ForeignKey("UserId")]
        public Customer Customer { get; set; }



    }
}
