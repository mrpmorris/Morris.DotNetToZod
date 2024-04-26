using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class RegularExpressionAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not RegularExpressionAttribute regexAttribute)
			return;

		string escapedRegexPattern = regexAttribute.Pattern.Replace("/", @"\/");
		builder.Write($".regex(/{escapedRegexPattern}/{CommaErrorMessageFor(attribute, propertyName)})");
	}
}

