namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    using System.ComponentModel.DataAnnotations;

    public class VendorDTO
    {
        [Required(ErrorMessage = "Vendor name is required.")]
        public string VendorName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }
    }

}
