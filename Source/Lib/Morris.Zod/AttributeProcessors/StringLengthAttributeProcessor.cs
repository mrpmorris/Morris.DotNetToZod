using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class StringLengthAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not StringLengthAttribute stringLengthAttribute)
			return;

		if (stringLengthAttribute.MinimumLength > 0)
			builder.Write($".min({stringLengthAttribute.MinimumLength}{CommaErrorMessageFor(attribute, propertyName)})");

		builder.Write($".max({stringLengthAttribute.MaximumLength}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

