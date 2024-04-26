using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class EmailAddressAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not EmailAddressAttribute)
			return;

		builder.Write($".email({ErrorMessageFor(attribute, propertyName)})");
	}
}

