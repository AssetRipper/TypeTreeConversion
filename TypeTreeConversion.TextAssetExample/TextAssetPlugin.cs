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
		//We want to convert all assets to TextAssets,
		//so we assign an alternative default field converter.
		registry.DefaultConverter = new TextAssetFieldConverter(registry);

		//This is cleared to ensure prior MonoBehaviours get TextAsset fields.
		//If it was not cleared, MonoBehaviourFieldConverter would be used instead.
		//There's generally no need to clear unless you're defining a custom default converter.
		registry.Converters.Clear();
	}

	private static void RegisterTypeTreeReplacers(TypeTreeReplacerRegistry registry)
	{
		//We want to replace all type trees with the type tree of TextAsset,
		//so we assign an alternative default replacer.
		registry.DefaultReplacer = new TextAssetTypeTreeReplacer(registry);

		//This is cleared to ensure prior MonoBehaviours get new TextAsset type trees.
		//If it was not cleared, MonoBehaviourTypeTreeReplacer would be used instead.
		//There's generally no need to clear unless you're defining a custom default replacer.
		registry.Replacers.Clear();
	}
}
