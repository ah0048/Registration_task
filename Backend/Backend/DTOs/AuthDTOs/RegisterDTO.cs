using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.DTOs.AuthDTOs
{
    public class RegisterDTO
    {
        [Required]
        [RegularExpression(
            @"^[\p{IsArabic}\s]{2,100}$",
            ErrorMessage = "Arabic name may only contain Arabic letters and spaces (2–100 chars)."
        )]
        public string CompanyNameAr { get; set; }
        [Required]
        [RegularExpression(
            @"^[A-Za-z\s]{2,100}$",
            ErrorMessage = "English name may only contain A–Z letters and spaces (2–100 chars)."
        )]
        public string CompanyNameEn { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        [Required, Url]
        public string WebsiteUrl { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
