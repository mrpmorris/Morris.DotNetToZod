using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;

namespace Morris.DotNetToZod.AttributeProcessors;

public class FileExtensionsAttributeProcessor : AttributeProcessorBase
{
	public override async Task ProcessAsync(IndentedTextWriter builder, string propertyName, AttributeInfo attributeInfo)
	{
		if (attributeInfo.FullName != typeof(FileExtensionsAttribute).FullName)
			return;

		var attribute = new FileExtensionsAttribute();
		attributeInfo.SetPropertyValues(attribute);

		var extensions = attribute.Extensions.Split(',');
		var regexPattern = $"\\.({string.Join("|", extensions)})$";
		await builder.WriteAsync($".regex(/'{regexPattern}'/{CommaErrorMessageFor(attribute, propertyName)}, {{ flags: 'i' }})");
	}
}

