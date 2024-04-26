using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class RangeAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(RangeAttribute).FullName)
			return;

		var attribute = new RangeAttribute(
			minimum: (int)attributeInfo.PropertyValues[nameof(RangeAttribute.Minimum)]!,
			maximum: (int)attributeInfo.PropertyValues[nameof(RangeAttribute.Maximum)]!
		);
		attributeInfo.SetPropertyValues(attribute);

		await builder.WriteAsync($".min({attribute.Minimum}{CommaErrorMessageFor(attribute, propertyName)})");
		await builder.WriteAsync($".max({attribute.Maximum}{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

