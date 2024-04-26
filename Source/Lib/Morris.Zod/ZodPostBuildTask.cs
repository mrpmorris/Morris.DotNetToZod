using Microsoft.Build.Framework;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Morris.Zod;

public class ZodPostBuildTask : Microsoft.Build.Utilities.Task
{
	public string AssemblyPath { get; set; } = null!;
	public string ProjectFilePath { get; set; } = null!;
	public string GeneratedTypeScriptOutputDir { get; set; } = null!;

	public override bool Execute()
	{
		Log.LogMessage(MessageImportance.High, $"Framework {RuntimeInformation.FrameworkDescription}");
		bool isValid = true;
		isValid = IsNotNullOrWhiteSpace(paramName: nameof(AssemblyPath), value: AssemblyPath) && isValid;
		isValid = IsNotNullOrWhiteSpace(paramName: nameof(ProjectFilePath), value: ProjectFilePath) && isValid;
		isValid = IsNotNullOrWhiteSpace(
			paramName: nameof(GeneratedTypeScriptOutputDir),
			value: GeneratedTypeScriptOutputDir,
			additionalInformation: @"The output directory can be specified in your csproj like so: <PropertyGroup><Morris_Zod_OutputDir>.\ZodGeneratedFiles</Morris_Zod_OutputDir></PropertyGroup>");

		if (!isValid)
			return false;

		GeneratedTypeScriptOutputDir = GeneratedTypeScriptOutputDir.Trim();
		if (!Path.IsPathRooted(GeneratedTypeScriptOutputDir))
		{
			Log.LogMessage(MessageImportance.High, "Using relative output directory");
			string projectFileDirectoryPath = Path.GetDirectoryName(ProjectFilePath!)!;
			GeneratedTypeScriptOutputDir = Path.Combine(projectFileDirectoryPath, GeneratedTypeScriptOutputDir);
		}
		Log.LogMessage(MessageImportance.High, $"Assembly loaded from: {AssemblyPath}");
		Log.LogMessage(MessageImportance.High, $"Project file path: {ProjectFilePath}");
		Log.LogMessage(MessageImportance.High, $"Custom output directory: {GeneratedTypeScriptOutputDir}");

		var generator = new TypeScriptSourceGenerator(
			assemblyPath: AssemblyPath,
			projectFilePath: ProjectFilePath,
			generatedTypeScriptOutputDir: GeneratedTypeScriptOutputDir,
			logMessage: x => Log.LogMessage(MessageImportance.High, x),
			logError: x => Log.LogError(x));

		Assembly assembly = AssemblyLoader.Load(AssemblyPath);
		generator.GenerateZodFiles(assembly, GeneratedTypeScriptOutputDir);

		return true;
	}

	private bool IsNotNullOrWhiteSpace(string paramName, string? value, string? additionalInformation = null)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			Log.LogError($"{paramName} must not be empty. {additionalInformation}");
			return false;
		}
		return true;
	}
}
