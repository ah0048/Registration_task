using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs.AuthDTOs
{
    public class SetPasswordDTO
    {
        [Required]
        public string Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 7, ErrorMessage = "Password must be at least 7 characters long.")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
