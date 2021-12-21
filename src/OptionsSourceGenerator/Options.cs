namespace OptionsSourceGenerator;

public sealed class Options : IEquatable<Options>
{
    public readonly string? RootNamespace;

    public Options(AnalyzerConfigOptions options)
    {
        if (!options.TryGetValue("build_property.RootNamespace", out RootNamespace))
        {
            RootNamespace = null;
        }
    }

    public Options(string? rootNamespace)
    {
        RootNamespace = rootNamespace;
    }

    public bool Equals(Options other) => RootNamespace == other.RootNamespace;

    public static Options Select(AnalyzerConfigOptionsProvider provider, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return new Options(provider.GlobalOptions);
    }
}