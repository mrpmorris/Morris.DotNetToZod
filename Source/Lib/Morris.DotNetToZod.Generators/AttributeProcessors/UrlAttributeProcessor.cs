using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class UrlAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(UrlAttribute).FullName)
			return;

		var attribute = new UrlAttribute();
		attributeInfo.SetPropertyValues(attribute);

		await builder.WriteAsync($".url({ErrorMessageFor(attribute, propertyName)})");
	}
}

