using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class RequiredAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(RequiredAttribute).FullName)
			return;

		var attribute = new RequiredAttribute();
		attributeInfo.SetPropertyValues(attribute);

		await builder.WriteAsync($".required({ErrorMessageFor(attribute, propertyName)})");

		if (!attribute.AllowEmptyStrings)
			await builder.WriteAsync($".nonempty({ErrorMessageFor(attribute, propertyName)})");
	}
}
