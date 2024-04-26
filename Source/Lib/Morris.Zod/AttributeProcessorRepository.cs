using Morris.Zod.AttributeProcessors;

namespace Morris.Zod;

public static class AttributeProcessorRepository
{
	public static readonly IReadOnlyList<AttributeProcessorBase> Processors = FindProcessors();

	private static IReadOnlyList<AttributeProcessorBase> FindProcessors() => typeof(AttributeProcessorRepository)
		.Assembly
		.GetTypes()
		.Where(x => typeof(AttributeProcessorBase).IsAssignableFrom(x))
		.Where(x => x.IsClass && !x.IsAbstract)
		.Select(Activator.CreateInstance)
		.Cast<AttributeProcessorBase>()
		.ToList()
		.AsReadOnly();
}
