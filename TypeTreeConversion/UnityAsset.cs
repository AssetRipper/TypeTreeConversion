using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics;

namespace TypeTreeConversion;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly record struct UnityAsset
{
	public AssetsManager Manager { get; }
	public AssetsFileInstance FileInstance { get; }
	public AssetFileInfo FileInfo { get; }

	public UnityAsset(AssetsManager manager, AssetsFileInstance file, AssetFileInfo info)
	{
		Manager = manager;
		FileInstance = file;
		FileInfo = info;
	}

	public SerializeFile File => new SerializeFile(FileInstance, Manager);

	public int TypeID
	{
		get => FileInfo.TypeId;
		set => FileInfo.TypeId = value;
	}

	public long PathID => FileInfo.PathId;

	public string Name
	{
		get
		{
			AssetTypeValueField baseField = BaseField;
			AssetTypeValueField nameField = baseField.Get("m_Name");
			string? name = nameField.IsDummy ? null : nameField.AsString;
			if (string.IsNullOrEmpty(name) && TypeID == 48)//Shader
			{
				name = baseField.Get("m_ParsedForm").Get("m_Name").AsString;
			}
			return name ?? "";
		}
	}

	public AssetTypeValueField BaseField
	{
		get
		{
			return Manager.GetBaseField(FileInstance, FileInfo);
		}
		set
		{
			FileInfo.SetNewData(value);
		}
	}

	private string GetDebuggerDisplay()
	{
		return $"{TypeID} : {Name}";
	}

	public UnityAsset? ResolveAsset(AssetTypeValueField pptrField)
	{
		AssetExternal assetExternal = Manager.GetExtAsset(FileInstance, pptrField);
		return assetExternal.file is null || assetExternal.info is null
			? null
			: new UnityAsset(Manager, assetExternal.file, assetExternal.info);
	}
}
