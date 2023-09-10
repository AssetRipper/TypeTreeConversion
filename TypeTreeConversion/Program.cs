using AssetsTools.NET;
using AssetsTools.NET.Extra;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace TypeTreeConversion;

public static class Program
{
	public static event Action<FieldConverterRegistry>? RegisterFieldConverters;
	public static event Action<TypeTreeReplacerRegistry>? RegisterTypeTreeReplacers;

	static void Main(string[] args)
	{
		RootCommand rootCommand = new() { Description = "TypeTree Conversion" };

		Option<FileInfo?> inputOption = new(
						aliases: new[] { "-i", "--input" },
						description: "",
						getDefaultValue: () => null);
		rootCommand.AddOption(inputOption);

		Option<FileInfo?> outputOption = new(
						aliases: new[] { "-o", "--output" },
						description: "",
						getDefaultValue: () => null);
		rootCommand.AddOption(outputOption);

		Option<FileInfo?> typeTreeOption = new(
						name: "--type-tree",
						description: "The type tree json for the input file.",
						getDefaultValue: () => null);
		rootCommand.AddOption(typeTreeOption);

		Option<FileInfo?> tpkOption = new(
						name: "--tpk",
						description: "The tpk file, available at https://github.com/AssetRipper/Tpk",
						getDefaultValue: () => null);
		rootCommand.AddOption(tpkOption);

		Option<List<FileInfo?>?> pluginOption = new(
						name: "--plugin",
						description: "A plugin to be loaded.",
						getDefaultValue: () => null);
		rootCommand.AddOption(pluginOption);

		rootCommand.SetHandler((FileInfo? input, FileInfo? output, FileInfo? typeTree, FileInfo? tpk, List<FileInfo?>? pluginList) =>
		{
			ValidateArguments(input, output, typeTree, tpk, pluginList);
			LoadPlugins(pluginList);
			Run(input.FullName, output.FullName, typeTree.FullName, tpk.FullName);
		},
		inputOption, outputOption, typeTreeOption, tpkOption, pluginOption);

		new CommandLineBuilder(rootCommand)
			.UseVersionOption()
			.UseHelp()
			.UseEnvironmentVariableDirective()
			.UseParseDirective()
			.UseSuggestDirective()
			.RegisterWithDotnetSuggest()
			.UseTypoCorrections()
			.UseParseErrorReporting()
#if !DEBUG
			.UseExceptionHandler()
#endif
			.CancelOnProcessTermination()
			.Build()
			.Invoke(args);
	}

	private static void Run(string inputPath, string outputPath, string typeTreePath, string tpkPath)
	{
		AssetsManager manager = new();
		manager.LoadClassPackage(new MemoryStream(TpkCreator.ConvertJsonToTpk(typeTreePath)));
		AssetsFileInstance file = manager.LoadAssetsFile(inputPath, false);
		ClassDatabaseFile sourceClassDatabase = manager.LoadClassDatabaseFromPackage(file.file.Metadata.UnityVersion);
		SerializeFile serializeFile = new(file, manager);

		ClassPackageFile tpk = new();
		tpk.Read(tpkPath);
		ClassDatabaseFile destinationClassDatabase = tpk.GetClassDatabase(file.file.Metadata.UnityVersion);

		FieldConverterRegistry registry = new(sourceClassDatabase, destinationClassDatabase);
		RegisterFieldConverters?.Invoke(registry);

		foreach (UnityAsset asset in serializeFile.Assets)
		{
			FieldConverter converter = registry.GetConverter(asset.TypeID);
			converter.Convert(asset);
		}

		TypeTreeReplacerRegistry replacerRegistry = new(sourceClassDatabase, destinationClassDatabase);
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

	private static void LoadPlugins(List<FileInfo?>? pluginList)
	{
		if (pluginList is not null)
		{
			foreach (FileInfo? plugin in pluginList)
			{
				Assembly assembly = Assembly.LoadFrom(plugin!.FullName);
				foreach (RegisterPluginAttribute attribute in assembly.GetCustomAttributes<RegisterPluginAttribute>())
				{
					attribute.CreateInstance();
				}
			}
		}
	}

	private static void ValidateArguments([NotNull] FileInfo? input, [NotNull] FileInfo? output, [NotNull] FileInfo? typeTree, [NotNull] FileInfo? tpk, List<FileInfo?>? pluginList)
	{
		ArgumentNullException.ThrowIfNull(input);
		ArgumentNullException.ThrowIfNull(output);
		ArgumentNullException.ThrowIfNull(typeTree);
		ArgumentNullException.ThrowIfNull(tpk);
		ThrowIfNotExists(input);
		ThrowIfNotExists(typeTree);
		ThrowIfNotExists(tpk);
		if (pluginList is not null)
		{
			foreach (FileInfo? plugin in pluginList)
			{
				ArgumentNullException.ThrowIfNull(plugin);
				ThrowIfNotExists(plugin);
			}
		}
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
