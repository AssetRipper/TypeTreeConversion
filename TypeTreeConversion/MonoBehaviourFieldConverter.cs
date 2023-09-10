namespace TypeTreeConversion;

/// <summary>
/// This replacer ensures that MonoBehaviours are correctly converted.
/// Currently, this is done by doing nothing.
/// </summary>
public class MonoBehaviourFieldConverter : DefaultFieldConverter
{
	public MonoBehaviourFieldConverter(FieldConverterRegistry registry) : base(registry)
	{
	}

	protected override bool ShouldConvert(UnityAsset asset) => false;
}
