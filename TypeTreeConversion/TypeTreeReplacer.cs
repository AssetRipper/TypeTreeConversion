using AssetsTools.NET;

namespace TypeTreeConversion;

public abstract class TypeTreeReplacer
{
	public TypeTreeType Replace(TypeTreeType original)
	{
		if (!ShouldReplace(original))
		{
			return original;
		}

		return CreateReplacement(original.TypeId) ?? original;
	}

	protected virtual bool ShouldReplace(TypeTreeType original) => true;

	protected abstract TypeTreeType? CreateReplacement(int originalTypeID);
}
