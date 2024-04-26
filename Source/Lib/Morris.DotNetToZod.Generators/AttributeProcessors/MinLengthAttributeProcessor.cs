using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class MinLengthAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(MinLengthAttribute).FullName)
			return;

		var attribute = new MinLengthAttribute(
			length: (int)attributeInfo.PropertyValues[nameof(MinLengthAttribute.Length)]!
		);
		attributeInfo.SetPropertyValues(attribute);

		await builder.WriteAsync($".min({attribute.Length}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

