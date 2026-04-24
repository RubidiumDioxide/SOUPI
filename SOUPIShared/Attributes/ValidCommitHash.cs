using System.ComponentModel.DataAnnotations;
using SOUPIShared.Extensions; 


namespace SOUPIShared.Attributes
{
    internal class ValidCommitHash : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value as string;

            if (!stringValue.IsValidCommitHash())
            {
                return new ValidationResult(ErrorMessage ?? "Значение свойства не соответствует формату хэша коммита.");
            }

            return ValidationResult.Success;
        }
    }
}
