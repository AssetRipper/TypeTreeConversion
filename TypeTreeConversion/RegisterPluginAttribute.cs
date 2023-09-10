namespace TypeTreeConversion;

public abstract class RegisterPluginAttribute : Attribute
{
	public abstract Type Type { get; }

	internal abstract ConversionPlugin CreateInstance();
}
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class RegisterPluginAttribute<T> : RegisterPluginAttribute where T : ConversionPlugin, new()
{
	public override Type Type => typeof(T);

	internal override T CreateInstance() => new();
}
