namespace ApiVersioningDemo.Attributes;

[AttributeUsage (AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class EvenNumberAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid (object? value, ValidationContext validationContext)
	{
		if (value is int intValue)
		{
			return intValue % 2 == 0
				? ValidationResult.Success
				: new ValidationResult (ErrorMessage ?? "The {0} must be an even number.");
		}

		return new ValidationResult (ErrorMessage ?? "The {0} must be a valid integer.");
	}
}