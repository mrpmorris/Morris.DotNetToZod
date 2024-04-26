using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Morris.DotNetToZod;

public class AttributeInfo
{
	public readonly string FullName;
	public readonly IReadOnlyDictionary<string, object?> PropertyValues;

	public AttributeInfo(string fullName, IReadOnlyDictionary<string, object?> propertyValues)
	{
		FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
		PropertyValues = propertyValues ?? throw new ArgumentNullException(nameof(propertyValues));
	}

	public void SetPropertyValues(ValidationAttribute attribute)
	{
		if (attribute is null)
			throw new ArgumentNullException(nameof(attribute));

		PropertyInfo[] propertyInfos =
			attribute
				.GetType()
				.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Where(x => x.CanWrite)
				.ToArray();

		foreach (PropertyInfo propertyInfo in propertyInfos)
		{
			if (PropertyValues.TryGetValue(propertyInfo.Name, out object? value))
				propertyInfo.SetValue(attribute, value);
		}
	}
}
