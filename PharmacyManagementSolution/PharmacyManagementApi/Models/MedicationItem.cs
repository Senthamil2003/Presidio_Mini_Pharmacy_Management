using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacyManagementApi.Models
{
    public class MedicationItem
    {
        [Key]
        public int MedicationItemId { get; set; }
        public int MedicineId { get; set; }
        public int MedicationId { get; set; }

        public int Quantity { get; set; }
        [ForeignKey("MedicineId")]
        public Medicine Medicine { get; set; }
        [ForeignKey("MedicationId")]
        public Medication Medication { get; set; }

    }
}
