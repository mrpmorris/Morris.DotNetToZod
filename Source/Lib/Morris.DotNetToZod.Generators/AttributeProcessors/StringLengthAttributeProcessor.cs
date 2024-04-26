using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class StringLengthAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(StringLengthAttribute).FullName)
			return;

		var attribute = new StringLengthAttribute(
			maximumLength: (int)attributeInfo.PropertyValues[nameof(StringLengthAttribute.MaximumLength)]!
		);
		attributeInfo.SetPropertyValues(attribute);

		if (attribute.MinimumLength > 0)
			await builder.WriteAsync($".min({attribute.MinimumLength}{CommaErrorMessageFor(attribute, propertyName)})");

		await builder.WriteAsync($".max({attribute.MaximumLength}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

