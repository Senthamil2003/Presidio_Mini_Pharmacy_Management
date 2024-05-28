﻿using System.ComponentModel.DataAnnotations;

namespace PharmacyManagementApi.Models.DTO.RequestDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 100 characters.")]
        public string Password { get; set; }
    }
}
