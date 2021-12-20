namespace OptionsSourceGenerator;

public sealed class Options : IEquatable<Options>
{
    public readonly string? RootNamespace;

    public readonly bool IsDesinTimeBuild;

    public Options(AnalyzerConfigOptions options)
    {
        IsDesinTimeBuild = options.TryGetValue("build_property.DesignTimeBuild", out var DesignTimeBuild) && DesignTimeBuild == "true";

        if (!options.TryGetValue("build_property.RootNamespace", out RootNamespace))
        {
            RootNamespace = null;
        }
    }

    public bool Equals(Options other) => IsDesinTimeBuild == other.IsDesinTimeBuild && RootNamespace == other.RootNamespace;

    public override bool Equals(object obj) => obj is Options other && Equals(other);

    public override int GetHashCode()
    {
        if (RootNamespace is null)
        {
            return IsDesinTimeBuild ? 1 : 0;
        }

        int hashCode = RootNamespace.GetHashCode();
        return IsDesinTimeBuild ? hashCode : -hashCode;
    }

    public static Options Select(AnalyzerConfigOptionsProvider provider, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        return new(provider.GlobalOptions);
    }
}
