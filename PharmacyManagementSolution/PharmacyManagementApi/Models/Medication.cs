using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }   
        public string MedicationName { get; set;}
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
        public ICollection<MedicationItem> MedicationItems { get; set; }



    }
}
