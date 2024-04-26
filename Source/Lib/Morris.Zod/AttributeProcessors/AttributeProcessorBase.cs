using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public abstract class AttributeProcessorBase
{
	public abstract void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName);

	protected string? ErrorMessageFor(ValidationAttribute validationAttribute, string propertyName) =>
		validationAttribute.ErrorMessage is null
		? null
		: $"\"{validationAttribute.FormatErrorMessage(propertyName)}\"";

	protected string? CommaErrorMessageFor(ValidationAttribute validationAttribute, string propertyName)
	{
		string? message = ErrorMessageFor(validationAttribute, propertyName);
		return message is null ? null : $", {message}";
	}
}
