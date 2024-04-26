using Mono.Cecil;
using Mono.Cecil.Rocks;
using Morris.DotNetToZod.AttributeProcessors;
using Morris.DotNetToZod.Extensions;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace Morris.DotNetToZod;

public class TypeScriptSourceGenerator
{

	private readonly string AssemblyPath;
	private readonly string GeneratedFileNameSuffix = ".morris.zod.ts";
	private readonly string GeneratedTypeScriptOutputDir;
	private readonly string ExpectedDefaultNamespace;
	private readonly Action<string> LogMessage;
	private readonly Action<string> LogError;

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

	private TypeScriptSourceGenerator(
		string assemblyPath,
		string generatedTypeScriptOutputDir,
		Action<string> logMessage,
		Action<string> logError)
	{
		AssemblyPath = assemblyPath;
		GeneratedTypeScriptOutputDir = generatedTypeScriptOutputDir;
		LogMessage = logMessage ?? throw new ArgumentNullException(nameof(logMessage));
		LogError = logError ?? throw new ArgumentNullException(nameof(logError));
		ExpectedDefaultNamespace = Path.GetFileNameWithoutExtension(AssemblyPath) + ".";
	}

	public static async Task GenerateZodFilesAsync(
		string assemblyPath,
		string generatedTypeScriptOutputDir,
		Action<string> logMessage,
		Action<string> logError)
	{
		var generator = new TypeScriptSourceGenerator(
			assemblyPath: assemblyPath,
			generatedTypeScriptOutputDir: generatedTypeScriptOutputDir,
			logMessage: logMessage,
			logError: logError);

		await generator.GenerateAsync();
	}


	public async Task GenerateAsync()
	{
		Directory.CreateDirectory(GeneratedTypeScriptOutputDir);

		string[] filePaths = Directory.GetFiles(GeneratedTypeScriptOutputDir, "*" + GeneratedFileNameSuffix);
		if (filePaths.Length > 0)
		{
			LogMessage($"Deleting {filePaths.Length} previously generated files");
			Parallel.ForEach(filePaths, path => File.Delete(path));
		}

		AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(AssemblyPath);
		await GenerateFilesForAssemblyAsync(assemblyDefinition);
	}

	private async Task GenerateFilesForAssemblyAsync(AssemblyDefinition modelAssembly)
	{
		LogMessage("Generating files");

		var types = modelAssembly.Modules.SelectMany(module => module.Types)
			.Where(type => type.IsClass && !type.IsAbstract)
			.Where(type => !type.FullName.StartsWith("<", StringComparison.Ordinal))
			.ToArray();

		if (types.Length == 0)
		{
			LogMessage("No classes found");
			return;
		}

		LogMessage($"Generating source for {types.Length} classes");
		await Task.WhenAll(types.Select(GenerateFileForTypeAsync));
	}

	private async Task GenerateFileForTypeAsync(TypeDefinition type)
	{
		string typeDisplayName = type.Name;
		string fileName = typeDisplayName;
		if (string.IsNullOrEmpty(fileName)) return;

		if (fileName.StartsWith(ExpectedDefaultNamespace, StringComparison.OrdinalIgnoreCase))
			fileName = fileName.Substring(ExpectedDefaultNamespace.Length);

		fileName += GeneratedFileNameSuffix;
		LogMessage($"Generating file {fileName}");

		string fullFilePath = Path.Combine(GeneratedTypeScriptOutputDir, fileName);

		using var streamWriter = new StreamWriter(fullFilePath);
		using var builder = new IndentedTextWriter(streamWriter, tabString: "\t");

		await builder.WriteLineAsync($"// File: {fileName}");
		await builder.WriteLineAsync($"// Source namespace: {type.Namespace}");
		await builder.WriteLineAsync($"// Source class name: {typeDisplayName}");
		await builder.WriteLineAsync();
		await builder.WriteLineAsync("""import { z } from "zod";""");
		await builder.WriteLineAsync();
		await builder.WriteLineAsync("export const modelToValidateSchema = z");
		using (builder.WriteIndentedBlock(".object({", "});"))
		{
			await WritePropertiesAsync(builder, type);
		}
	}

	private async Task WritePropertiesAsync(IndentedTextWriter builder, TypeDefinition type)
	{
		var properties = type.Properties
			.Where(prop =>
				prop.GetMethod is not null
				&& prop.SetMethod is not null
				&& prop.GetMethod.IsPublic
				&& prop.SetMethod.IsPublic)
			.ToArray();

		foreach (var property in properties)
			await WritePropertyAsync(builder, property);
	}

	private async Task WritePropertyAsync(IndentedTextWriter builder, PropertyDefinition property)
	{
		string zodType = GetZodTypeValidatorName(property.PropertyType)!;
		await builder.WriteAsync($"{property.Name.ToCamelCase()}: z{zodType}");
		await WriteAttributesAsync(builder, property);
		await builder.WriteLineAsync(",");
	}

	private async Task WriteAttributesAsync(IndentedTextWriter builder, PropertyDefinition property)
	{
		string propertyName = property.Name.ToCamelCase();
		var attributes = property.CustomAttributes
			.Where(attr => attr.AttributeType.Resolve().IsDescendedFrom("System.ComponentModel.DataAnnotations.ValidationAttribute"))
			.ToArray();

		foreach (var attribute in attributes)
			await WriteAttributeAsync(builder, attribute, propertyName);
	}

	private async Task WriteAttributeAsync(IndentedTextWriter builder, CustomAttribute attribute, string propertyName)
	{
		AttributeInfo attributeInfo = attribute.ToAttributeInfo();
		foreach (AttributeProcessorBase processor in AttributeProcessorRepository.Processors)
			await processor.ProcessAsync(builder, propertyName, attributeInfo);
	}

	private string GetZodTypeValidatorName(TypeReference propertyType)
	{
		TypeDefinition resolvedPropertyType = propertyType.Resolve();
		return
			resolvedPropertyType.IsValueType && propertyType.IsNullable()
				? $"{GetZodTypeValidatorName(propertyType.GetNullableUnderlyingType()!)}.nullable()"
			: resolvedPropertyType.FullName == "System.String" ? ".string()"
			: resolvedPropertyType.IsNumber() ? ".number()"
			: resolvedPropertyType.FullName == "System.Numerics.BigInteger" ? ".bigint()"
			: resolvedPropertyType.FullName == "System.Boolean" ? ".boolean()"
			: resolvedPropertyType.IsDateTimeLike() ? ".date()"
			: resolvedPropertyType.FullName == "System.TimeOnly" ? ".time()"
			: resolvedPropertyType.IsArray || resolvedPropertyType.IsCollection() ? ".array()"
			: resolvedPropertyType.IsDictionary() ? ".record()"
			: resolvedPropertyType.IsSet() ? ".set()"
			: resolvedPropertyType.IsEnum ? ".enum()"
			: resolvedPropertyType.IsTuple() ? ".tuple()"
			: resolvedPropertyType.FullName == "System.Object" ? ".object()"
			: ".unknown()";
	}
}

