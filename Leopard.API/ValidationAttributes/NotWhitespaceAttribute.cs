using System.ComponentModel.DataAnnotations;

namespace Leopard.API.ValidationAttributes
{
	public class NotWhitespaceAttribute : ValidationAttribute
	{
		// TODO: Let swagger display custome validation attribute
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			if (!(value is string))
				return new ValidationResult("Must be string.");
			var s = value as string;
			if (string.IsNullOrWhiteSpace(s))
				return new ValidationResult("Satisfies string.IsNullOrWhiteSpace().");
			return ValidationResult.Success;
		}

		public override string ToString()
		{
			return "ss";
		}
	}
}
