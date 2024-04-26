using Morris.DotNetToZod.AttributeProcessors;
using Morris.DotNetToZod.Extensions;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Morris.DotNetToZod;

internal class TypeScriptSourceGenerator
{
	private const string GeneratedFileNameSuffix = ".morris.zod.ts";

	private readonly string AssemblyPath;
	private readonly string ProjectFilePath;
	private readonly string GeneratedTypeScriptOutputDir;
	private readonly Action<string> LogMessage;
	private readonly Action<string> LogError;
	private readonly string ExpectedDefaultNamespace;

	private readonly static AttributeProcessorBase[] AttributeProcessors = typeof(TypeScriptSourceGenerator)
		.Assembly
		.GetTypes()
		.Where(x => typeof(AttributeProcessorBase).IsAssignableFrom(x))
		.Where(x => x.IsClass)
		.Where(x => !x.IsAbstract)
		.Where(x => !x.IsGenericTypeDefinition)
		.Select(Activator.CreateInstance)
		.Cast<AttributeProcessorBase>()
		.ToArray();

	public TypeScriptSourceGenerator(
		string assemblyPath,
		string projectFilePath,
		string generatedTypeScriptOutputDir,
		Action<string> logMessage,
		Action<string> logError)
	{
		AssemblyPath = assemblyPath;
		ProjectFilePath = projectFilePath;
		GeneratedTypeScriptOutputDir = generatedTypeScriptOutputDir;
		LogMessage = logMessage ?? throw new ArgumentNullException(nameof(logMessage));
		LogError = logError ?? throw new ArgumentNullException(nameof(logError));
		ExpectedDefaultNamespace = Path.GetFileNameWithoutExtension(ProjectFilePath) + ".";
	}

	public void GenerateZodFiles(Assembly modelAssembly, string outputDirectoryPath)
	{
		Directory.CreateDirectory(outputDirectoryPath);

		string[] filePaths = Directory.GetFiles(outputDirectoryPath, "*" + GeneratedFileNameSuffix);
		if (filePaths.Length > 0)
		{
			LogMessage($"Deleting {filePaths.Length} previously generated files");
			Parallel.ForEach(filePaths, path => File.Delete(path));
		}

		try
		{
			GenerateFilesForAssembly(modelAssembly, outputDirectoryPath);
		}
		catch (ReflectionTypeLoadException ex)
		{
			var builder = new StringBuilder();
			foreach (Type? type in ex.Types)
				builder.Append($"Type: {type?.GetDisplayName()}");
			foreach (Exception? exception in ex.LoaderExceptions)
				builder.AppendLine($"Exception: {exception?.Message}");
			throw new ReflectionTypeLoadException(
				classes: ex.Types,
				exceptions: ex.LoaderExceptions,
				message: builder.ToString());
		}

	}

	private void GenerateFilesForAssembly(Assembly modelAssembly, string outputDirectoryPath)
	{
		LogMessage("Generating files");

		Type[] types = modelAssembly
			.GetTypes()
			.Where(x => x.IsClass && !x.IsAbstract)
			.ToArray();

		if (types.Length == 0)
		{
			LogMessage("No classes found");
			return;
		}

		LogMessage($"Generating source for {types.Length} classes");
		Parallel.ForEach(types, type => GenerateFileForType(type, outputDirectoryPath));
	}

	private void GenerateFileForType(Type type, string outputDirectoryPath)
	{
		string typeDisplayName = type.GetDisplayName();
		string fileName = typeDisplayName;
		if (string.IsNullOrEmpty(fileName)) return;

		if (fileName.StartsWith(ExpectedDefaultNamespace, StringComparison.OrdinalIgnoreCase))
			fileName = fileName.Substring(ExpectedDefaultNamespace.Length);

		fileName += GeneratedFileNameSuffix;
		LogMessage($"Generating file {fileName}");

		string fullFilePath = Path.Combine(outputDirectoryPath, fileName);

		using var streamWriter = new StreamWriter(fullFilePath);
		using var builder = new IndentedTextWriter(streamWriter, tabString: "\t");

		builder.WriteLine($"// File: {fileName}");
		builder.WriteLine($"// Source namespace: {type.Namespace}");
		builder.WriteLine($"// Source class name: {typeDisplayName}");
		builder.WriteLine();
		builder.WriteLine("""import { z } from "zod";""");
		builder.WriteLine();
		builder.WriteLine("export const modelToValidateSchema = z");
		using (builder.WriteIndentedBlock(".object({", "});"))
		{
			WriteProperties(builder, type);
		}
	}

	private void WriteProperties(IndentedTextWriter builder, Type type)
	{
		PropertyInfo[] properties = type
			.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
			.Where(x => x.CanRead && x.CanWrite)
			.ToArray();

		foreach (PropertyInfo property in properties)
			WriteProperty(builder, property);
	}

	private void WriteProperty(IndentedTextWriter builder, PropertyInfo property)
	{
		string zodType = GetZodTypeValidatorName(property.PropertyType)!;
		builder.Write($"{property.Name.ToCamelCase()}: z{zodType}");
		WriteAttributes(builder, property);
		builder.WriteLine(",");
	}

	private void WriteAttributes(IndentedTextWriter builder, PropertyInfo property)
	{
		string propertyName = property.Name.ToCamelCase();
		ValidationAttribute[] attributes = property.GetCustomAttributes<ValidationAttribute>(inherit: true).ToArray();
		foreach (ValidationAttribute attribute in attributes)
			WriteAttribute(builder, attribute, propertyName);
	}

	private void WriteAttribute(IndentedTextWriter builder, ValidationAttribute attribute, string propertyName)
	{
		foreach (AttributeProcessorBase processor in AttributeProcessorRepository.Processors)
			processor.Process(builder, attribute, propertyName);
	}

	private string GetZodTypeValidatorName(Type propertyType) =>
		propertyType.IsValueType && Nullable.GetUnderlyingType(propertyType) is Type innerType ? $"{GetZodTypeValidatorName(innerType)}.nullable()"
		: propertyType == typeof(string) ? ".string()"
		: propertyType == typeof(int)
			|| propertyType == typeof(double)
			|| propertyType == typeof(decimal)
			|| propertyType == typeof(float)
			|| propertyType == typeof(long)
			|| propertyType == typeof(short)
			|| propertyType == typeof(byte)
			|| propertyType == typeof(uint)
			|| propertyType == typeof(ulong)
			|| propertyType == typeof(ushort)
			|| propertyType == typeof(sbyte)
			? ".number()"
		: propertyType == typeof(System.Numerics.BigInteger) ? ".bigint()"
		: propertyType == typeof(bool) ? ".boolean()"
		: propertyType == typeof(DateTime)
			|| propertyType == typeof(DateTimeOffset)
#if NET6_0_OR_GREATER
			|| propertyType == typeof(DateOnly)
#endif
			? ".date()"
#if NET6_0_OR_GREATER
		: propertyType == typeof(TimeOnly) ? ".time()"
#endif
		: propertyType == typeof(Array)
			|| propertyType.IsArray
			|| propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(List<>)
			? ".array()"
		: propertyType == typeof(Dictionary<,>)
			|| propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>)
			|| propertyType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
			? ".record()"
		: propertyType == typeof(HashSet<>)
			|| propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ISet<>) ? ".set()"
		: propertyType.IsEnum ? ".enum()"
		: propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Tuple<,>)
			|| propertyType.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
			? ".tuple()"
		: propertyType == typeof(object) ? ".object()"
		: ".unknown()";
}
