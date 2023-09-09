﻿using AssetsTools.NET;

namespace TypeTreeConversionDemo;

public abstract class TypeTreeReplacer
{
	public TypeTreeType Replace(TypeTreeType original)
	{
		if (!ShouldReplace(original))
		{
			return original;
		}

		return CreateReplacement(original.TypeId);
	}

	protected virtual bool ShouldReplace(TypeTreeType original) => true;

	protected abstract TypeTreeType CreateReplacement(int originalTypeID);
}
