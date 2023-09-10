using AssetsTools.NET;

namespace TypeTreeConversion;

public abstract class Registry
{
	/// <summary>
	/// The class database of the source file, loaded from json.
	/// </summary>
	public ClassDatabaseFile SourceClassDatabase { get; }

	/// <summary>
	/// The class database of the destination file, loaded from a tpk.
	/// </summary>
	public ClassDatabaseFile DestinationClassDatabase { get; }

	internal Registry(ClassDatabaseFile sourceClassDatabase, ClassDatabaseFile destinationClassDatabase)
	{
		SourceClassDatabase = sourceClassDatabase;
		DestinationClassDatabase = destinationClassDatabase;
	}
}
