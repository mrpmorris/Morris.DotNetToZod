using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class MaxLengthAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not MaxLengthAttribute maxLengthAttribute)
			return;

		builder.Write($".max({maxLengthAttribute.Length}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

