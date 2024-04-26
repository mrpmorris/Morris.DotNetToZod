namespace Morris.Zod.Extensions;

internal static class StringToCamelCaseExtension
{
	public static string ToCamelCase(this string value) =>
		value switch {
			null => null!,
			string v when v.Length == 1 => v.ToLower(),
			string v when v.Length > 1 => char.ToLower(v[0]) + v.Substring(1),
			_ => value
		};
}
