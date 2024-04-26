using Mono.Cecil;

namespace Morris.DotNetToZod.Extensions;

internal static class CecilTypeReferenceExtensions
{
	public static bool IsNullable(this TypeReference typeRef)
	{
		var genericInstance = typeRef as GenericInstanceType;
		return genericInstance != null &&
			   genericInstance.ElementType.FullName == "System.Nullable`1" &&
			   genericInstance.GenericArguments.Count == 1;
	}

	public static TypeReference GetNullableUnderlyingType(this TypeReference typeRef)
	{
		if (typeRef.IsNullable())
		{
			var genericInstance = (GenericInstanceType)typeRef;
			return genericInstance.GenericArguments[0];
		}
		throw new NotImplementedException();
	}

}
