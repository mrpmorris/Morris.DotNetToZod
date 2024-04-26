using CommandLine;

namespace Morris.DotNetToZod.CommandLine;

public class Options
{
	[Value(0, MetaName = "Assembly", Required = true, HelpText = "Assembly file to convert.")]
	public string Assembly { get; set; } = null!;

	[Value(1, MetaName = "destinationFolder", Required = true, HelpText = "Destination folder.")]
	public string DestinationFolder { get; set; } = null!;

	[Option('s', "suffix", Default = "morris.zod.ts", HelpText = "Suffix to use at end of generated files. Also used when delete-mode=suffix.")]
	public string Suffix { get; set; } = null!;

	[Option("delete-files-suffixes", HelpText = "Semi-colon list of file suffixes to delete before generating source. Used instead of --suffix when --delete-mode=suffix.")]
	public string? DeleteFilesSuffix { get; set; }
}
