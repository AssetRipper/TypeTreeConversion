using AssetsTools.NET;

namespace TypeTreeConversion.TextAssetExample;

/// <summary>
/// Replaces all type trees with the type tree of TextAsset.
/// </summary>
public class TextAssetTypeTreeReplacer : DefaultTypeTreeReplacer
{
	public TextAssetTypeTreeReplacer(TypeTreeReplacerRegistry registry) : base(registry)
	{
	}

	protected override TypeTreeType? CreateReplacement(int originalTypeID)
	{
		return base.CreateReplacement(49);
	}
}
