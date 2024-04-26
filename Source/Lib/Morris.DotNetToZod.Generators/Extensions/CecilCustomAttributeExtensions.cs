using Mono.Cecil;
using Mono.Collections.Generic;

namespace Morris.DotNetToZod.Extensions;

internal static class CecilCustomAttributeExtensions
{
	public static AttributeInfo ToAttributeInfo(this CustomAttribute attribute)
	{
		var properties = new Dictionary<string, object?>();

		MethodDefinition constructor = attribute.Constructor.Resolve();

		Collection<ParameterDefinition> parameters = constructor.Parameters;
		Collection<CustomAttributeArgument> arguments = attribute.ConstructorArguments;

		for (int i = 0; i < arguments.Count; i++)
		{
			CustomAttributeArgument arg = arguments[i];
			string paramName = parameters[i].Name.ToPascalCase();
			properties.Add(paramName, arg.Value);
		}

		foreach (var namedArg in attribute.Properties)
			properties.Add(namedArg.Name, namedArg.Argument.Value);

		return new AttributeInfo(attribute.AttributeType.FullName, properties);  // Assuming an appropriate constructor or factory method exists
	}
}
