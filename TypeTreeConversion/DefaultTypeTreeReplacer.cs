using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversion;

public class DefaultTypeTreeReplacer : TypeTreeReplacer
{
	private readonly ClassDatabaseFile classDatabase;

	public DefaultTypeTreeReplacer(ClassDatabaseFile classDatabase)
	{
		this.classDatabase = classDatabase;
	}

	protected override TypeTreeType? CreateReplacement(int originalTypeID)
	{
		return ClassDatabaseToTypeTree.Convert(classDatabase, originalTypeID);
	}
}
