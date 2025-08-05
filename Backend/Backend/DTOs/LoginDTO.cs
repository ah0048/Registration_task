using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
