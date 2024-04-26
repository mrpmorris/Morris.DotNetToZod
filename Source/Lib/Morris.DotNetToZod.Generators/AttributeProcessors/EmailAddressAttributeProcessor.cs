using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class EmailAddressAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(EmailAddressAttribute).FullName)
			return;

		var attribute = new EmailAddressAttribute();
		attributeInfo.SetPropertyValues(attribute);
		await builder.WriteAsync($".email({ErrorMessageFor(attribute, propertyName)})");
	}
}

