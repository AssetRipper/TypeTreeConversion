using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversion;

public readonly struct SerializeFile
{
	private readonly AssetsFileInstance file;
	private readonly AssetsManager manager;

	public SerializeFile(AssetsFileInstance file, AssetsManager manager)
	{
		this.file = file;
		this.manager = manager;
	}

	public IEnumerable<UnityAsset> Assets
	{
		get
		{
			foreach (AssetFileInfo info in file.file.AssetInfos)
			{
				yield return new UnityAsset(manager, file, info);
			}
		}
	}

	public static SerializeFile Load(string path)
	{
		AssetsManager manager = new();
		AssetsFileInstance file = manager.LoadAssetsFile(path, false);
		return new SerializeFile(file, manager);
	}
}
