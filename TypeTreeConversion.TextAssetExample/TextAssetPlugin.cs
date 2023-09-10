namespace TypeTreeConversion.TextAssetExample;

/// <summary>
/// This example plugin replaces all assets with TextAssets.
/// </summary>
public class TextAssetPlugin : ConversionPlugin
{
	public TextAssetPlugin()
	{
		Program.RegisterFieldConverters += RegisterFieldConverters;
		Program.RegisterTypeTreeReplacers += RegisterTypeTreeReplacers;
	}

	private static void RegisterFieldConverters(FieldConverterRegistry registry)
	{
		TextAssetFieldConverter converter = new(registry.ClassDatabase);
		registry.Converters.Clear();
		registry.DefaultConverter = converter;
	}

	private static void RegisterTypeTreeReplacers(TypeTreeReplacerRegistry registry)
	{
		TextAssetTypeTreeReplacer replacer = new(registry.ClassDatabase);
		registry.Replacers.Clear();
		registry.DefaultReplacer = replacer;
	}
}
