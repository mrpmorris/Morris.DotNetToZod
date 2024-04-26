using Morris.Zod.Extensions;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Morris.Zod;

internal static class AssemblyLoader
{
#if NET6_0_OR_GREATER
	public static Assembly Load(string assemblyPath) => Assembly.LoadFile(assemblyPath);
#endif

#if NET472_OR_GREATER
	private static readonly ConcurrentDictionary<string, bool> AssemblyDirectories = new ConcurrentDictionary<string, bool>();

	public static Assembly Load(string assemblyPath)
	{
		AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
		AssemblyDirectories[Path.GetDirectoryName(assemblyPath)!] = true;
		return Assembly.LoadFile(assemblyPath);
	}

	private static Assembly? ResolveAssembly(object? sender, ResolveEventArgs args)
	{
		string dependentAssemblyName = args.Name.Split(',')[0] + ".dll";
		List<string> directoriesToScan = AssemblyDirectories.Keys.ToList();

		foreach (string directoryToScan in directoriesToScan)
		{
			string dependentAssemblyPath = Path.Combine(directoryToScan, dependentAssemblyName);
			if (File.Exists(dependentAssemblyPath))
				return Load(dependentAssemblyPath);
		}
		return null;
	}
#endif
}