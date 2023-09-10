using AssetsTools.NET;

namespace TypeTreeConversion;

public sealed class TypeTreeReplacerRegistry : Registry
{
	/// <summary>
	/// The replacers to use for each type ID.
	/// </summary>
	/// <remarks>
	/// This initially contains a <see cref="MonoBehaviourTypeTreeReplacer"/> instance for type ID 114.
	/// </remarks>
	public Dictionary<int, TypeTreeReplacer> Replacers { get; } = new();

	/// <summary>
	/// The default replacer to use when no replacer is found for a type ID.
	/// </summary>
	/// <remarks>
	/// This is initially set to a <see cref="DefaultTypeTreeReplacer"/> instance.
	/// </remarks>
	public TypeTreeReplacer DefaultReplacer { get; set; }

	internal TypeTreeReplacerRegistry(ClassDatabaseFile sourceClassDatabase, ClassDatabaseFile destinationClassDatabase) : base(sourceClassDatabase, destinationClassDatabase)
	{
		DefaultReplacer = new DefaultTypeTreeReplacer(this);
		Replacers.Add(114, new MonoBehaviourTypeTreeReplacer(this));
	}

	public TypeTreeReplacer GetReplacer(int typeID)
	{
		return Replacers.TryGetValue(typeID, out TypeTreeReplacer? converter) ? converter : DefaultReplacer;
	}
}
