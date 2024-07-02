using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PharmacyManagementApi.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackId { get; set; }
        public string FeedbackTitle { get; set; }
        public string FeedbackMessage { get; set; }
        public int MedicineId { get; set; }
        public int CustomerId { get; set; }
        public double Rating { get; set; }
        public DateTime Date { get; set; }

        [ForeignKey("CustomerId")]
        [JsonIgnore]
        public Customer Customer { get; set; }

        [ForeignKey("MedicineId")]
        [JsonIgnore]
        public Medicine Medicine { get; set; }
    }
}
