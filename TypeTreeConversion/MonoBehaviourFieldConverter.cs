using AssetsTools.NET;

namespace TypeTreeConversion;

public class MonoBehaviourFieldConverter : DefaultFieldConverter
{
	public MonoBehaviourFieldConverter(ClassDatabaseFile classDatabase) : base(classDatabase)
	{
	}

	protected override bool ShouldConvert(UnityAsset asset) => false;
}
