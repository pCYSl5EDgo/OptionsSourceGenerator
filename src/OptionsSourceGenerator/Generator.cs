namespace OptionsSourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var properties = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .SelectMany(Utility.SelectCompilerVisibleProperty)
            .WithComparer(StringComparer.Ordinal)
            .Collect();

        var options = context.AnalyzerConfigOptionsProvider
            .Select(Options.Select)
            .WithComparer(EqualityComparer<Options>.Default);

        context.RegisterSourceOutput(properties.Combine(options), static (context, pair) =>
        {
            var (properties, options) = pair;
            var text = Utility.GenerateSource(properties, options, context.CancellationToken);
            context.AddSource("Options.cs", text);
        });
    }
}
