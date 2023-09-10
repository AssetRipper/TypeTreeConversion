using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversion;

public class DefaultFieldConverter : FieldConverter
{
	private readonly ClassDatabaseFile classDatabase;

	public DefaultFieldConverter(FieldConverterRegistry registry)
	{
		classDatabase = registry.DestinationClassDatabase;
	}

	protected override AssetTypeValueField? CreateNewBaseField(int originalTypeID)
	{
		AssetTypeTemplateField templateField = new();
		ClassDatabaseType? cldbType = classDatabase.FindAssetClassByID(originalTypeID);
		if (cldbType is null)
		{
			return null;
		}
		templateField.FromClassDatabase(classDatabase, cldbType);
		return ValueBuilder.DefaultValueFieldFromTemplate(templateField);
	}
}
