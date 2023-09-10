using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics;

namespace TypeTreeConversion;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct UnityAsset
{
	public AssetsManager Manager { get; }
	public AssetsFileInstance File { get; }
	public AssetFileInfo Info { get; }

	public UnityAsset(AssetsManager manager, AssetsFileInstance file, AssetFileInfo info)
	{
		this.Manager = manager;
		this.File = file;
		this.Info = info;
	}

	public int TypeID
	{
		get => Info.TypeId;
		set => Info.TypeId = value;
	}

	public long PathID => Info.PathId;

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
			return Manager.GetBaseField(File, Info);
		}
		set
		{
			Info.SetNewData(value);
		}
	}

	private string GetDebuggerDisplay()
	{
		return $"{TypeID} : {Name}";
	}

	public UnityAsset? ResolveAsset(AssetTypeValueField pptrField)
	{
		AssetExternal assetExternal = Manager.GetExtAsset(File, pptrField);
		return assetExternal.file is null || assetExternal.info is null
			? null
			: new UnityAsset(Manager, assetExternal.file, assetExternal.info);
	}
}
