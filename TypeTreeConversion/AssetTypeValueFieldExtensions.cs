using AssetsTools.NET;
using System.Diagnostics.CodeAnalysis;

namespace TypeTreeConversion;

public static class AssetTypeValueFieldExtensions
{
	public static AssetValueType GetValueType(this AssetTypeValueField field)
	{
		return field.Value is null ? AssetValueType.None : field.Value.ValueType;
	}

	public static bool HasNoChildren(this AssetTypeValueField field)
	{
		return field.Children is null or { Count: 0 };
	}

	public static bool TryGetChild(this AssetTypeValueField parent, string childName, [NotNullWhen(true)] out AssetTypeValueField? child)
	{
		foreach (AssetTypeValueField field in parent.Children)
		{
			if (field.FieldName == childName)
			{
				child = field;
				return true;
			}
		}
		child = null;
		return false;
	}
}
