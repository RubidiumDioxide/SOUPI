using System.ComponentModel.DataAnnotations;
using SOUPIShared.Extensions; 


namespace SOUPIShared.Attributes
{
    internal class ValidGitHubUsername : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value as string;

            if (string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.Success;
            }

            if (!stringValue.IsValidGitHubUsername())
            {
                return new ValidationResult(ErrorMessage ?? "Значение свойства не соответствует формату имени пользователя GitHub.");
            }

            return ValidationResult.Success;
        }
    }
}
