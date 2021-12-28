using System.Text;

namespace OptionsSourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var globalProperties = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(Utility.SelectGlobalCompilerVisibleProperty)
            .Where(x => x is { Item1: not null, Item2.Length: > 0 });

        var additionalFilesProperties = context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Select(Utility.SelectAdditionalCompilerVisibleItemMetadata)
            .Where(x => x is { Item1: not null, Item2.Length: > 0 });

        var options = context.AnalyzerConfigOptionsProvider
            .Select(Options.Select)
            .WithComparer(EqualityComparer<Options>.Default);

        context.RegisterSourceOutput(globalProperties.Combine(options), static (context, pair) =>
        {
            var ((name, properties), options) = pair;
            var template = new GlobalOptionsTemplate(options.RootNamespace, name, null, properties);
            var builder = new StringBuilder();
            template.TransformAppend(builder);
            var text = builder.ToString();
            var hintName = builder.Clear().Append(name).Append(".global.cs").ToString();
            context.AddSource(hintName, text);
        });

        context.RegisterSourceOutput(additionalFilesProperties.Combine(options), static (context, pair) =>
        {
            var ((name, properties), options) = pair;
            var template = new AdditionalFileOptionsTemplate(options.RootNamespace, name, null, properties);
            var builder = new StringBuilder();
            template.TransformAppend(builder);
            var text = builder.ToString();
            var hintName = builder.Clear().Append(name).Append(".additional.cs").ToString();
            context.AddSource(hintName, text);
        });
    }
}
