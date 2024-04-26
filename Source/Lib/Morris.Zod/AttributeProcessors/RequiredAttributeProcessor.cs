using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class RequiredAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not RequiredAttribute requiredAttribute)
			return;

		builder.Write($".required({ErrorMessageFor(attribute, propertyName)})");

		if (!requiredAttribute.AllowEmptyStrings)
			builder.Write($".nonempty({ErrorMessageFor(attribute, propertyName)})");
	}
}
