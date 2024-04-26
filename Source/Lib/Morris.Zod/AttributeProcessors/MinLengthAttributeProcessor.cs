using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class MinLengthAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not MinLengthAttribute minLengthAttribute)
			return;

		builder.Write($".min({minLengthAttribute.Length}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

