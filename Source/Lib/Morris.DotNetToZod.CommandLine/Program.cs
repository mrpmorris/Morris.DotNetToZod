using CommandLine;
using Morris.DotNetToZod;
using Morris.DotNetToZod.CommandLine;

await Parser.Default.ParseArguments<Options>(args).WithParsedAsync(ExecuteAsync);

async Task ExecuteAsync(Options options)
{
	await TypeScriptSourceGenerator
		.GenerateZodFilesAsync(
			assemblyPath: options.Assembly,
			generatedTypeScriptOutputDir: options.DestinationFolder,
			logMessage: Console.WriteLine,
			logError: LogError);
}

void LogError(string error)
{
	(Console.ForegroundColor, Console.BackgroundColor) = (Console.BackgroundColor, Console.ForegroundColor);
	Console.WriteLine(error);
	Console.ResetColor();
}