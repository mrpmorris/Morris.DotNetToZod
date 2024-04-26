namespace Morris.Zod.Extensions;

internal static class TypeGetDisplayNameExtension
{
	public static string GetDisplayName(this Type t)
	{
		if (!t.IsGenericType)
			return t.FullName!;
		string genericTypeName = t.GetGenericTypeDefinition().FullName!.Split('`')[0];
		string genericArgumentTypes = string.Join(",", t.GetGenericArguments().Select(GetDisplayName));
		return $"{genericTypeName}<{genericArgumentTypes}>";
	}
}