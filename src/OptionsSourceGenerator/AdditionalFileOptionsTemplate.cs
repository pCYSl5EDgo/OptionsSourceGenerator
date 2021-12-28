namespace OptionsSourceGenerator;

public partial class AdditionalFileOptionsTemplate
{
    public readonly string Namespace;

    public readonly string Name;
    
    public readonly string FieldName;

    public readonly string Modifier;

    public readonly ImmutableArray<string> Properties;

    public AdditionalFileOptionsTemplate(string? @namespace, string? name, string? fieldName, string? modifier, ImmutableArray<string> properties)
    {
        Namespace = @namespace ?? "OptionsSourceGenerator";
        Name = name ?? "AdditionalFileOptions";
        FieldName = fieldName ?? "Text";
        Modifier = modifier ?? "public sealed partial class";
        Properties = properties;
    }
}
