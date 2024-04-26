using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public abstract class AttributeProcessorBase
{
	public abstract Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo);

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
