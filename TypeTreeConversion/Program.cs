using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace TypeTreeConversion;

public static class Program
{
	public static event Action<FieldConverterRegistry>? RegisterFieldConverters;
	public static event Action<TypeTreeReplacerRegistry>? RegisterTypeTreeReplacers;

	static void Main(string[] args)
	{
		RootCommand rootCommand = new() { Description = "TypeTree Conversion" };

		Option<FileInfo?> inputOption = new Option<FileInfo?>(
						aliases: new[] { "-i", "--input" },
						description: "",
						getDefaultValue: () => null);
		rootCommand.AddOption(inputOption);

		Option<FileInfo?> outputOption = new Option<FileInfo?>(
						aliases: new[] { "-o", "--output" },
						description: "",
						getDefaultValue: () => null);
		rootCommand.AddOption(outputOption);

		Option<FileInfo?> typeTreeOption = new Option<FileInfo?>(
						name: "--type-tree",
						description: "The type tree json for the input file.",
						getDefaultValue: () => null);
		rootCommand.AddOption(typeTreeOption);

		Option<FileInfo?> tpkOption = new Option<FileInfo?>(
						name: "--tpk",
						description: "The tpk file, available at https://github.com/AssetRipper/Tpk",
						getDefaultValue: () => null);
		rootCommand.AddOption(tpkOption);

		rootCommand.SetHandler((FileInfo? input, FileInfo? output, FileInfo? typeTree, FileInfo? tpk) =>
		{
			ValidateArguments(input, output, typeTree, tpk);
			Run(input.FullName, output.FullName, typeTree.FullName, tpk.FullName);
		},
		inputOption, outputOption, typeTreeOption, tpkOption);

		new CommandLineBuilder(rootCommand)
			.UseDefaults()
			.Build()
			.Invoke(args);
	}

	private static void Run(string inputPath, string outputPath, string typeTreePath, string tpkPath)
	{
		AssetsManager manager = new();
		manager.LoadClassPackage(new MemoryStream(TpkCreator.ConvertJsonToTpk(typeTreePath)));
		AssetsFileInstance file = manager.LoadAssetsFile(inputPath, false);
		manager.LoadClassDatabaseFromPackage(file.file.Metadata.UnityVersion);
		SerializeFile serializeFile = new(file, manager);

		ClassPackageFile tpk = new();
		tpk.Read(tpkPath);
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

		file.Write(outputPath);

		Console.WriteLine("Done!");
	}

	private static void ValidateArguments([NotNull] FileInfo? input, [NotNull] FileInfo? output, [NotNull] FileInfo? typeTree, [NotNull] FileInfo? tpk)
	{
		ArgumentNullException.ThrowIfNull(input);
		ArgumentNullException.ThrowIfNull(output);
		ArgumentNullException.ThrowIfNull(typeTree);
		ArgumentNullException.ThrowIfNull(tpk);
		ThrowIfNotExists(input);
		ThrowIfNotExists(typeTree);
		ThrowIfNotExists(tpk);
	}

	private static void ThrowIfNotExists(FileInfo file, [CallerArgumentExpression(nameof(file))] string? paramName = null)
	{
		if (!file.Exists)
		{
			throw new ArgumentException($"File {file.FullName} does not exist.", paramName);
		}
	}

	private static void Write(this AssetsFileInstance file, string path)
	{
		using FileStream stream = File.Create(path);
		AssetsFileWriter writer = new(stream);
		file.file.Write(writer);
	}
}
