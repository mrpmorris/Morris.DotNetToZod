using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class MaxLengthAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(MaxLengthAttribute).FullName)
			return;

		var attribute = new MaxLengthAttribute();
		attributeInfo.SetPropertyValues(attribute);

		await builder.WriteAsync($".max({attribute.Length}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

