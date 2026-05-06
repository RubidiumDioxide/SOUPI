using System.ComponentModel.DataAnnotations;
using SOUPIShared.Extensions; 


namespace SOUPIShared.Attributes
{
    internal class ConsistsOfNumbersCyrillicLatin : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var stringValue = value as string;

            if (stringValue == null)
            {
                return ValidationResult.Success;
            }

            if (!stringValue.DoesConsistOfNumbersCyrillicLatin())
            {
                return new ValidationResult(ErrorMessage ?? "Свойство может содержать только буквы русского языка, латинские буквы, цифры и пробел.");
            }

            return ValidationResult.Success;
        }
    }
}
