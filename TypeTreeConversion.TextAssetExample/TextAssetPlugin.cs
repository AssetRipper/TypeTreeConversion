namespace TypeTreeConversion.TextAssetExample;

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
		TextAssetTypeTreeConverter converter = new(registry.ClassDatabase);
		registry.Replacers.Clear();
		registry.DefaultReplacer = converter;
	}
}
