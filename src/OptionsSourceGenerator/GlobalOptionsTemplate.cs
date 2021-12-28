namespace OptionsSourceGenerator;

public partial class GlobalOptionsTemplate
{
    public readonly string Namespace;

    public readonly string Name;

    public readonly string Modifier;

    public readonly ImmutableArray<string> Properties;

    public GlobalOptionsTemplate(string? @namespace, string? name, string? modifier, ImmutableArray<string> properties)
    {
        Namespace = @namespace ?? "OptionsSourceGenerator";
        Name = name ?? "GlobalOptions";
        Modifier = modifier ?? "public sealed partial class";
        Properties = properties;
    }
}
