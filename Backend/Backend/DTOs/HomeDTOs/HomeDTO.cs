using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.HomeDTOs
{
    public class HomeDTO
    {
        [Required]
        public string CompanyNameEn { get; set; }
        public string? LogoUrl { get; set; }
    }
}
