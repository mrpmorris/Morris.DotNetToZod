using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class RegularExpressionAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(RegularExpressionAttribute).FullName)
			return;

		var attribute = new RegularExpressionAttribute(
			pattern: (string)attributeInfo.PropertyValues[nameof(RegularExpressionAttribute.Pattern)]!
		);
		attributeInfo.SetPropertyValues(attribute);

		string escapedRegexPattern = attribute.Pattern.Replace("/", @"\/");
		await builder.WriteAsync($".regex(/{escapedRegexPattern}/{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

