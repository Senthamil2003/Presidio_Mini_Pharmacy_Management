using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class PurchaseItem
    {
        [Required(ErrorMessage = "Vendor name is required.")]
        public int VendorId { get; set; }

        [Required(ErrorMessage = "Medicine name is required.")]
        public int  MedicineId { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public int Amount { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Expiry date is required.")]
        [FutureDate(ErrorMessage = "Expiry date must be in the future.")]
        public DateTime ExpiryDate { get; set; }

    }
    [ExcludeFromCodeCoverage]
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime)
            {
                DateTime dateValue = (DateTime)value;
                return dateValue > DateTime.Now;
            }
            return false;
        }
    }


}
