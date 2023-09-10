using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversion;

public class DefaultTypeTreeReplacer : TypeTreeReplacer
{
	private readonly ClassDatabaseFile classDatabase;

	public DefaultTypeTreeReplacer(TypeTreeReplacerRegistry registry)
	{
		classDatabase = registry.DestinationClassDatabase;
	}

	protected override TypeTreeType? CreateReplacement(int originalTypeID)
	{
		return ClassDatabaseToTypeTree.Convert(classDatabase, originalTypeID);
	}
}
