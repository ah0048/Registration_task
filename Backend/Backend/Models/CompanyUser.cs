using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Backend.Models
{
    public class CompanyUser: IdentityUser
    {
        [Required]
        public string CompanyNameAr { get; set; }
        [Required]
        public string CompanyNameEn { get; set; }
        public string? LogoUrl { get; set; }
        [Required, Url]
        public string WebsiteUrl { get; set; }
    }
}
