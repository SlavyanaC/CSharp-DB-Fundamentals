namespace P01_BillsPaymentSystem.Data.Models.Attributes
{
    using System.ComponentModel.DataAnnotations;

    public class NonUnicodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string nullErrorMsg = "Value cannot be null!";

            if (value == null)
            {
                return new ValidationResult(nullErrorMsg);
            }

            string text = (string)value;
            string notUniCodeCharMsg = "Value cannot contain unicode character!";

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] > 255)
                {
                    return new ValidationResult(notUniCodeCharMsg);
                }
            }

            return ValidationResult.Success;
        }
    }
}
