using AssetsTools.NET;

namespace TypeTreeConversionDemo;

public class MonoBehaviourTypeTreeReplacer : DefaultTypeTreeReplacer
{
	public MonoBehaviourTypeTreeReplacer(ClassDatabaseFile classDatabase) : base(classDatabase)
	{
	}

	protected override bool ShouldReplace(TypeTreeType original) => false;
}
