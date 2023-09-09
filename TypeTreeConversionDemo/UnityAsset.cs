using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.Diagnostics;

namespace TypeTreeConversionDemo;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public readonly struct UnityAsset
{
	private readonly AssetsManager manager;
	private readonly AssetsFileInstance file;
	private readonly AssetFileInfo info;

	public UnityAsset(AssetsManager manager, AssetsFileInstance file, AssetFileInfo info)
	{
		this.manager = manager;
		this.file = file;
		this.info = info;
	}

	public int TypeID
	{
		get => info.TypeId;
		set => info.TypeId = value;
	}

	public long PathID => info.PathId;

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
			return manager.GetBaseField(file, info);
		}
		set
		{
			info.SetNewData(value);
		}
	}

	private string GetDebuggerDisplay()
	{
		return $"{TypeID} : {Name}";
	}
}
