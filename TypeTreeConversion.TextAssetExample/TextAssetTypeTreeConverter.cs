using AssetsTools.NET;

namespace TypeTreeConversion.TextAssetExample;

public class TextAssetTypeTreeConverter : DefaultTypeTreeReplacer
{
	public TextAssetTypeTreeConverter(ClassDatabaseFile classDatabase) : base(classDatabase)
	{
	}

	protected override TypeTreeType? CreateReplacement(int originalTypeID)
	{
		return base.CreateReplacement(49);
	}
}
