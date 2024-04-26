//using System.Text.Json;
//using System.Text.Json.Serialization;

//namespace Morris.Zod.PlaygroundStuff.Json;

//internal class RuntimeTypeJsonConverter : JsonConverter<object?>
//{
//	public override bool CanConvert(Type typeToConvert) => typeToConvert.FullName == "System.RuntimeType";

//	public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//	{
//		throw new NotImplementedException();
//	}

//	public override void Write(Utf8JsonWriter writer, object? value, JsonSerializerOptions options)
//	{
//	}
//}