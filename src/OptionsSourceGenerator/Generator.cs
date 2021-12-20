namespace OptionsSourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var options = context.AnalyzerConfigOptionsProvider
            .Select(Options.Select)
            .WithComparer(EqualityComparer<Options>.Default);

        var properties = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .SelectMany(Utility.SelectCompilerVisibleProperty)
            .WithComparer(StringComparer.Ordinal)
            .Collect();

        context.RegisterSourceOutput(options.Combine(properties), static (context, pair) =>
        {
            context.AddSource("Options.cs", Utility.GenerateSource(pair.Left, pair.Right, context.CancellationToken));
        });
    }
}
