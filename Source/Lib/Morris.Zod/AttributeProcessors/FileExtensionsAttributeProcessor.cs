using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.Zod.AttributeProcessors;

public class FileExtensionsAttributeProcessor : AttributeProcessorBase
{
	public override void Process(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		if (attribute is not FileExtensionsAttribute fileExtensionsAttribute)
			return;

		var extensions = fileExtensionsAttribute.Extensions.Split(',');
		var regexPattern = $"\\.({string.Join("|", extensions)})$";
		builder.Write($".regex(/'{regexPattern}'/{CommaErrorMessageFor(attribute, propertyName)}, {{ flags: 'i' }})");
	}
}

