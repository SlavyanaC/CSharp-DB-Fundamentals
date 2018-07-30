namespace P01_BillsPaymentSystem.Data.Models.Attributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property)]
    public class XorAttribute : ValidationAttribute
    {
        private readonly string xorTargetAttribute;

        public XorAttribute(string xorTargetAttribute)
        {
            this.xorTargetAttribute = xorTargetAttribute;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var targetAttribute = validationContext.ObjectType
                .GetProperty(xorTargetAttribute)
                .GetValue(validationContext.ObjectInstance);

            if ((targetAttribute == null && value != null) || (targetAttribute != null && value == null))
            {
                return ValidationResult.Success;
            }

            string errorMsg = "The two properties must have opposite values!";
            return new ValidationResult(errorMsg);
        }
    }
}
