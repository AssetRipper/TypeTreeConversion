using AssetsTools.NET;

namespace TypeTreeConversion;

public class MonoBehaviourTypeTreeReplacer : DefaultTypeTreeReplacer
{
	public MonoBehaviourTypeTreeReplacer(ClassDatabaseFile classDatabase) : base(classDatabase)
	{
	}

	protected override bool ShouldReplace(TypeTreeType original) => false;
}
