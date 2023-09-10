using AssetsTools.NET;

namespace TypeTreeConversion.TextAssetExample;

/// <summary>
/// Replaces all fields with the fields of TextAsset.
/// </summary>
public class TextAssetFieldConverter : DefaultFieldConverter
{
	public TextAssetFieldConverter(ClassDatabaseFile classDatabase) : base(classDatabase)
	{
	}

	protected override AssetTypeValueField? CreateNewBaseField(int originalTypeID)
	{
		return base.CreateNewBaseField(49);
	}

	protected override void CopyFields(UnityAsset asset, AssetTypeValueField source, AssetTypeValueField destination)
	{
		CopyFieldsExactly(source, destination);
		destination["m_Script"].AsString = $"""
			This asset has been replaced with a text asset.
			Type ID: {asset.TypeID}
			Path ID: {asset.PathID}
			Name: {asset.Name}
			""";
	}
}
