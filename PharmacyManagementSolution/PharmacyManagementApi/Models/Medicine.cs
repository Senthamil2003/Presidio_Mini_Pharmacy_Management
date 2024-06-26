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
        public int CurrentQuantity { get; set; }
        private double _sellingPrice;
        public double FeedbackSum { get; set; }
        public double FeedbackCount { get; set; }
        public double SellingPrice { get; set; } = 0;
        public int TotalNumberOfPurchase { get; set; }
        public string? Description { get; set; }
        public byte[]? Image { get; set; }
        public int status { get; set; } = 0;
        public int BrandId { get; set; }
        public double RecentPurchasePrice { get; set; } = 0;

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
       

    }
}
