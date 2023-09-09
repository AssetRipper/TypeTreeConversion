using AssetsTools.NET;

namespace TypeTreeConversion;

public sealed class TypeTreeReplacerRegistry
{
	public Dictionary<int, TypeTreeReplacer> Replacers { get; }

	public TypeTreeReplacer DefaultReplacer { get; set; }

	public ClassDatabaseFile ClassDatabase { get; }

	internal TypeTreeReplacerRegistry(ClassDatabaseFile classDatabase)
	{
		ClassDatabase = classDatabase;
		DefaultReplacer = new DefaultTypeTreeReplacer(classDatabase);
		Replacers = new()
		{
			{ 114, new MonoBehaviourTypeTreeReplacer(classDatabase) }
		};
	}

	public TypeTreeReplacer GetReplacer(int typeID)
	{
		return Replacers.TryGetValue(typeID, out TypeTreeReplacer? converter) ? converter : DefaultReplacer;
	}
}