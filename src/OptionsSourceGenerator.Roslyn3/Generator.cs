using System.Collections.Immutable;

namespace OptionsSourceGenerator.Roslyn3;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();
        var analyzerConfigOptions = context.AnalyzerConfigOptions;
        SortedSet<string> set = new(StringComparer.OrdinalIgnoreCase);
        foreach (var file in context.AdditionalFiles)
        {
            foreach (var property in Utility.SelectCompilerVisibleProperty((file, analyzerConfigOptions), cancellationToken))
            {
                set.Add(property);
            }
        }

        var source = Utility.GenerateSource(set.ToImmutableArray(), cancellationToken);
        context.AddSource("Options.cs", source);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
    }
}
