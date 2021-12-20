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

        context.RegisterSourceOutput(properties, static (context, properties) =>
        {
            var text = Utility.GenerateSource(properties, context.CancellationToken);
            context.AddSource("Options.cs", text);
        });
    }
}
