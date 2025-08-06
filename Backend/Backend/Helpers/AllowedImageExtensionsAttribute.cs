using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Backend.Helpers
{
    public class AllowedImageExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly long _maxSizeInBytes;

        public AllowedImageExtensionsAttribute(string extensions = ".jpg,.jpeg,.png,.gif,.bmp,.webp", long maxSizeInMB = 5)
        {
            _extensions = extensions.Split(',');
            _maxSizeInBytes = maxSizeInMB * 1024 * 1024;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is not IFormFile file)
                return new ValidationResult("Invalid file type.");

            
            if (file.Length == 0)
                return new ValidationResult("File cannot be empty.");

            
            if (file.Length > _maxSizeInBytes)
                return new ValidationResult($"File size cannot exceed {_maxSizeInBytes / (1024 * 1024)} MB.");

            
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !_extensions.Contains(extension))
            {
                return new ValidationResult($"Only the following image extensions are allowed: {string.Join(", ", _extensions)}");
            }

            
            var allowedMimeTypes = new[]
            {
                "image/jpeg", "image/jpg", "image/png", "image/gif", 
                "image/bmp", "image/webp", "image/svg+xml"
            };

            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
            {
                return new ValidationResult("File must be a valid image format.");
            }

            return ValidationResult.Success;
        }
    }
}
