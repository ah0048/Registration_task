using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CheckOtpDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string OtpCode { get; set; }
    }
}
