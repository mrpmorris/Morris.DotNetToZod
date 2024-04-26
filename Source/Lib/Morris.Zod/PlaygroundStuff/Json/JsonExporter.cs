//using System.Reflection;
//using System.Text.Json;
//using static Morris.Zod.TypeScriptSourceGenerator;

//namespace Morris.Zod.PlaygroundStuff.Json;

//internal static class JsonExporter
//{
//	private readonly static JsonSerializerOptions DefaultJsonSerializerOptions;

//	static JsonExporter()
//	{
//		DefaultJsonSerializerOptions =
//			new JsonSerializerOptions {
//				WriteIndented = true
//			};
//		DefaultJsonSerializerOptions.Converters.Add(new RuntimeTypeJsonConverter());
//	}

//	public static void WriteTypeToFile(string fullFilePath, Type type)
//	{
//		var typeDescription =
//			new
//			{
//				BaseType = type.BaseType?.GetDisplayName(),
//				type.Namespace,
//				type.Name,
//				Attributes = GetAttributes(type),
//				Properties = GetProperties(type)
//			};

//		string json = JsonSerializer.Serialize(typeDescription, DefaultJsonSerializerOptions);
//		File.WriteAllText(path: fullFilePath, contents: json);
//	}

//	static IEnumerable<object> GetAttributes(MemberInfo member) =>
//		member
//			.GetCustomAttributes(false)
//			.Select(attr => new
//			{
//				AttributeType = attr.GetType().GetDisplayName(),
//				Properties = GetAttributeProperties(attr)
//			});

//	static Dictionary<string, object> GetAttributeProperties(object attribute) =>
//		attribute
//			.GetType()
//			.GetProperties(BindingFlags.Public | BindingFlags.Instance)
//			.Where(x => x.GetValue(attribute) is not null)
//			.ToDictionary(
//				x => x.Name,
//				x => ConvertPropertyForSerialization(x.GetValue(attribute)!)
//			);

//	static object ConvertPropertyForSerialization(object value) =>
//		value is Type type
//		? type.GetDisplayName()
//		: value;

//	static IEnumerable<object> GetProperties(Type type) =>
//		type
//		.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
//		.Select(x => new
//		{
//			PropertyName = x.Name,
//			PropertyType = x.PropertyType.GetDisplayName(),
//			IsStatic = x.GetAccessors().Any(x => x.IsStatic),
//			Attributes = GetAttributes(x).ToList()
//		});
//}
