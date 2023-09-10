using AssetsTools.NET;

namespace TypeTreeConversion;

/// <summary>
/// This replacer ensures that MonoBehaviours are correctly converted.
/// Currently, this is done by doing nothing.
/// </summary>
public class MonoBehaviourTypeTreeReplacer : DefaultTypeTreeReplacer
{
	public MonoBehaviourTypeTreeReplacer(TypeTreeReplacerRegistry registry) : base(registry)
	{
	}

	protected override bool ShouldReplace(TypeTreeType original) => false;
}
