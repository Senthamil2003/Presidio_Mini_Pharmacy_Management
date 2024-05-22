using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Medicine
    {
        [Key]
        public int  MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int CategoryId {  get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
       


    }
}
