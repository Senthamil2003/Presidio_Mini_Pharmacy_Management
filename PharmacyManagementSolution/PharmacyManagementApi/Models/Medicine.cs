﻿using System.ComponentModel.DataAnnotations;
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
        public double SellingPrice
        {
            get { return _sellingPrice; }
            set { _sellingPrice = value + 20; }
        }
        public int TotalNumberOfPurchase { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        public ICollection<Feedback>? Feedbacks { get; set; }
       


    }
}
