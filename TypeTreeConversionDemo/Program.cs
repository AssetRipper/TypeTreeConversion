using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace TypeTreeConversionDemo;

internal static class Program
{
	//Arg 0: Path to assets file
	//Arg 1: Path to save converted assets file
	//Arg 2: Path to json type tree
	//Arg 3: Path to unity tpk
	static void Main(string[] args)
	{
		AssetsManager manager = new();
		manager.LoadClassPackage(new MemoryStream(TpkCreator.ConvertJsonToTpk(args[2])));
		AssetsFileInstance file = manager.LoadAssetsFile(args[0], false);
		manager.LoadClassDatabaseFromPackage(file.file.Metadata.UnityVersion);
		SerializeFile serializeFile = new(file, manager);

		ClassPackageFile tpk = new();
		tpk.Read(args[3]);
		ClassDatabaseFile unityClassDatabase = tpk.GetClassDatabase(file.file.Metadata.UnityVersion);

		foreach (UnityAsset asset in serializeFile.Assets)
		{
			AssetTypeValueField baseField;
			{
				AssetTypeTemplateField templateField = new();
				templateField.FromClassDatabase(unityClassDatabase, unityClassDatabase.FindAssetClassByID(asset.TypeID));
				baseField = ValueBuilder.DefaultValueFieldFromTemplate(templateField);
			}

			CopyFields(asset.BaseField, baseField);
			
			asset.BaseField = baseField;
		}

		for (int i = 0; i < file.file.Metadata.TypeTreeTypes.Count; i++)
		{
			TypeTreeType original = file.file.Metadata.TypeTreeTypes[i];
			if (original.TypeId is not 114)
			{
				TypeTreeType replacement = ClassDatabaseToTypeTree.Convert(unityClassDatabase, original.TypeId);
				file.file.Metadata.TypeTreeTypes[i] = replacement;
			}
		}

		using (FileStream stream = File.Create(args[1]))
		{
			AssetsFileWriter writer = new(stream);
			file.file.Write(writer);
		}

		Console.WriteLine("Done!");
	}

	private static void CopyFields(AssetTypeValueField source, AssetTypeValueField destination)
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
				CopyFields(sourceChild, destinationChild);
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
						CopyFields(sourceGrandchild, destinationGrandchild);
					}
				}
			}
		}
	}

	private static AssetValueType GetValueType(this AssetTypeValueField field)
	{
		return field.Value is null ? AssetValueType.None : field.Value.ValueType;
	}

	private static bool HasNoChildren(this AssetTypeValueField field)
	{
		return field.Children is null or { Count: 0 };
	}

	private static bool TryGetChild(this AssetTypeValueField parent, string childName, [NotNullWhen(true)] out AssetTypeValueField? child)
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
