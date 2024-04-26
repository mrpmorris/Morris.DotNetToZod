using Mono.Cecil;

namespace Morris.DotNetToZod.Extensions;

internal static class CecilTypeDefinitionExtensions
{
	private readonly static string[] TupleClassNames =
	[
		"System.Tuple`2",
		"System.ValueTuple`2"
	];

	private readonly static string[] DictionaryClassNames =
	[
		"System.Collections.Generic.IDictionary`2",
		"System.Collections.Generic.Dictionary`2"
	];

	public static bool IsNumber(this TypeDefinition type) => 
		type.FullName == "System.Int32"
		|| type.FullName == "System.Double"
		|| type.FullName == "System.Decimal"
		|| type.FullName == "System.Float"
		|| type.FullName == "System.Int64"
		|| type.FullName == "System.Int16"
		|| type.FullName == "System.Byte"
		|| type.FullName == "System.UInt32"
		|| type.FullName == "System.UInt64"
		|| type.FullName == "System.UInt16"
		|| type.FullName == "System.SByte";

	public static bool IsDateTimeLike(this TypeDefinition type) =>
		type.FullName == "System.DateTime"
		|| type.FullName == "System.DateTimeOffset"
		|| type.FullName == "System.DateOnly";

	public static bool IsArray(this TypeDefinition type) => type.IsArray || IsCollection(type);

	public static bool IsCollection(this TypeDefinition type) =>
		type.Interfaces?
			.Any(i => i.InterfaceType.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal)) ?? false;

	public static bool IsDictionary(this TypeDefinition type) =>
		type.Interfaces?.Any(i => DictionaryClassNames.Contains(i.InterfaceType.FullName, StringComparer.Ordinal)) == true;

	public static bool IsSet(this TypeDefinition type) =>
		type.Interfaces?.Any(i => i.InterfaceType.FullName.StartsWith("System.Collections.Generic.ISet`1", StringComparison.Ordinal)) ?? false;

	public static bool IsTuple(this TypeDefinition type) =>
		TupleClassNames.Contains(type.FullName)
		|| (type.Interfaces?.Any(i => TupleClassNames.Contains(i.InterfaceType.FullName, StringComparer.Ordinal)) ?? false);

	public static bool IsDescendedFrom(this TypeDefinition type, string className)
	{
		TypeDefinition? currentType = type;
		while (currentType is not null && currentType.FullName != "System.Object")
		{
			if (currentType.FullName == className)
				return true;

			if (currentType.BaseType is not null)
				currentType = currentType.BaseType.Resolve();
			else
				currentType = null;
		}
		return false;
	}
}
