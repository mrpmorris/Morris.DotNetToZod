using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class UrlAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not UrlAttribute)
			return;

		builder.Write($".url({ErrorMessageFor(attribute, propertyName)})");
	}
}

