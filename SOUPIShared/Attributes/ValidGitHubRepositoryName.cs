using System.ComponentModel.DataAnnotations;
using SOUPIShared.Extensions; 


namespace SOUPIShared.Attributes
{
    internal class ValidGitHubRepositoryName : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value as string;

            if (!stringValue.IsValidGitHubRepositoryName())
            {
                return new ValidationResult(ErrorMessage ?? "Значение свойства не соответствует формату названия репозитория GitHub.");
            }

            return ValidationResult.Success;
        }
    }
}
