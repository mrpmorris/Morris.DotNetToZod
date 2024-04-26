using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class RangeAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not RangeAttribute rangeAttribute)
			return;

		builder.Write($".min({rangeAttribute.Minimum}{CommaErrorMessageFor(attribute, propertyName)})");
		builder.Write($".max({rangeAttribute.Maximum}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

