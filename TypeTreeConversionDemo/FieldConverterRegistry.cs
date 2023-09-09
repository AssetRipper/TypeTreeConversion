using AssetsTools.NET;

namespace TypeTreeConversionDemo;

public sealed class FieldConverterRegistry
{
	public Dictionary<int, FieldConverter> Converters { get; }

	public FieldConverter DefaultConverter { get; set; }

	public ClassDatabaseFile ClassDatabase { get; }

	internal FieldConverterRegistry(ClassDatabaseFile classDatabase)
	{
		ClassDatabase = classDatabase;
		DefaultConverter = new DefaultFieldConverter(classDatabase);
		Converters = new()
		{
			{ 114, new MonoBehaviourFieldConverter(classDatabase) }
		};
	}

	public FieldConverter GetConverter(int typeID)
	{
		return Converters.TryGetValue(typeID, out FieldConverter? converter) ? converter : DefaultConverter;
	}
}
