using AssetsTools.NET;

namespace TypeTreeConversion;

public sealed class FieldConverterRegistry : Registry
{
	/// <summary>
	/// The converters to use for each type ID.
	/// </summary>
	/// <remarks>
	/// This initially contains a <see cref="MonoBehaviourFieldConverter"/> instance for type ID 114.
	/// </remarks>
	public Dictionary<int, FieldConverter> Converters { get; } = new();

	/// <summary>
	/// The default converter to use when no converter is found for a type ID.
	/// </summary>
	/// <remarks>
	/// This is initially set to a <see cref="DefaultFieldConverter"/> instance.
	/// </remarks>
	public FieldConverter DefaultConverter { get; set; }

	internal FieldConverterRegistry(ClassDatabaseFile sourceClassDatabase, ClassDatabaseFile destinationClassDatabase) : base(sourceClassDatabase, destinationClassDatabase)
	{
		DefaultConverter = new DefaultFieldConverter(this);
		Converters.Add(114, new MonoBehaviourFieldConverter(this));
	}

	public FieldConverter GetConverter(int typeID)
	{
		return Converters.TryGetValue(typeID, out FieldConverter? converter) ? converter : DefaultConverter;
	}
}
