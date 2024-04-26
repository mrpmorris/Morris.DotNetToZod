namespace Morris.DotNetToZod.Extensions;

internal static class StringExtensions
{
	public static string ToCamelCase(this string value) =>
		TransformFirstCharacter(value, char.ToLower);

	public static string ToPascalCase(this string value) =>
		TransformFirstCharacter(value, char.ToUpper);

	private static string TransformFirstCharacter(string value, Func<char, char> transform) =>
		value switch {
			null => null!,
			string v when v.Length == 1 => transform(v[0]).ToString(),
			string v when v.Length > 1 => transform(v[0]) + v.Substring(1),
			_ => value
		};
}
