using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TypeTreeConversionDemo;

public static class Program
{
	public static event Action<FieldConverterRegistry>? RegisterFieldConverters;
	public static event Action<TypeTreeReplacerRegistry>? RegisterTypeTreeReplacers;

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

		FieldConverterRegistry registry = new(unityClassDatabase);
		RegisterFieldConverters?.Invoke(registry);

		foreach (UnityAsset asset in serializeFile.Assets)
		{
			FieldConverter converter = registry.GetConverter(asset.TypeID);
			converter.Convert(asset);
		}

		TypeTreeReplacerRegistry replacerRegistry = new(unityClassDatabase);
		RegisterTypeTreeReplacers?.Invoke(replacerRegistry);

		for (int i = 0; i < file.file.Metadata.TypeTreeTypes.Count; i++)
		{
			TypeTreeType original = file.file.Metadata.TypeTreeTypes[i];
			TypeTreeReplacer replacer = replacerRegistry.GetReplacer(original.TypeId);
			file.file.Metadata.TypeTreeTypes[i] = replacer.Replace(original);
		}

		file.Write(args[1]);

		Console.WriteLine("Done!");
	}

	private static void Write(this AssetsFileInstance file, string path)
	{
		using FileStream stream = File.Create(path);
		AssetsFileWriter writer = new(stream);
		file.file.Write(writer);
	}
}
