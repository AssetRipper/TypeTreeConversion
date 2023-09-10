using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics;

namespace TypeTreeConversion;

public abstract class FieldConverter
{
	protected virtual bool ShouldConvert(UnityAsset asset) => true;

	public void Convert(UnityAsset asset)
	{
		if (!ShouldConvert(asset))
		{
			return;
		}

		AssetTypeValueField? baseField = CreateNewBaseField(asset.TypeID);

		if (baseField is null)
		{
			return;
		}

		CopyFields(asset, asset.BaseField, baseField);

		asset.BaseField = baseField;
	}

	protected abstract AssetTypeValueField? CreateNewBaseField(int originalTypeID);

	protected virtual void CopyFields(UnityAsset asset, AssetTypeValueField source, AssetTypeValueField destination)
	{
		CopyFieldsExactly(source, destination);
	}

	protected static void CopyFieldsExactly(AssetTypeValueField source, AssetTypeValueField destination)
	{
		foreach (AssetTypeValueField destinationChild in destination.Children)
		{
			if (!source.TryGetChild(destinationChild.FieldName, out AssetTypeValueField? sourceChild))
			{
				continue;
			}

			AssetValueType destinationType = destinationChild.GetValueType();
			AssetValueType sourceType = sourceChild.GetValueType();
			if (destinationType != sourceType)
			{
				continue;
			}
			if (destinationType == AssetValueType.None)
			{
				CopyFieldsExactly(sourceChild, destinationChild);
			}
			else
			{
				destinationChild.Value = new AssetTypeValue(sourceChild.Value.ValueType, sourceChild.Value.AsObject);
				if (!sourceChild.HasNoChildren())
				{
					Debug.Assert(destinationChild.HasNoChildren());
					destinationChild.Children ??= new();
					foreach (AssetTypeValueField sourceGrandchild in sourceChild.Children)
					{
						AssetTypeValueField destinationGrandchild = ValueBuilder.DefaultValueFieldFromArrayTemplate(destinationChild.TemplateField);
						destinationChild.Children.Add(destinationGrandchild);
						CopyFieldsExactly(sourceGrandchild, destinationGrandchild);
					}
				}
			}
		}
	}
}
