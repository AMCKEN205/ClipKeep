using System.ComponentModel.DataAnnotations;

namespace ClipKeep.Models.CustomModelValidators
{
    /// <summary>
    /// Check to see if a password contains the required special characters on registration.
    /// </summary>
    public class ContainsSpecialCharactersAttribute : ValidationAttribute
    {
        private readonly char[] _specialChars = "!@#$%^&*()<>".ToCharArray();

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // We'll only be getting string values from the password entry textbox so this should be a safe cast.
            var valueString = (string) value;
            var containsSpecialChars = valueString.IndexOfAny(_specialChars) == -1;

            if (containsSpecialChars)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            // Contains special characters 
            return null;
        }

    }
}