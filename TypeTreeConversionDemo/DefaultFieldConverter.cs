using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversionDemo;

public class DefaultFieldConverter : FieldConverter
{
	private readonly ClassDatabaseFile classDatabase;

	public DefaultFieldConverter(ClassDatabaseFile classDatabase)
	{
		this.classDatabase = classDatabase;
	}

	protected override AssetTypeValueField CreateNewBaseField(int originalTypeID)
	{
		AssetTypeTemplateField templateField = new();
		templateField.FromClassDatabase(classDatabase, classDatabase.FindAssetClassByID(originalTypeID));
		return ValueBuilder.DefaultValueFieldFromTemplate(templateField);
	}
}
