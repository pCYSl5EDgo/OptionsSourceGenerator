using System.Text;

namespace OptionsSourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class Generator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;
        cancellationToken.ThrowIfCancellationRequested();
        var analyzerConfigOptions = context.AnalyzerConfigOptions;
        var options = Options.Select(analyzerConfigOptions, cancellationToken);
        var builder = new StringBuilder();
        string? field;
        string? text;
        string? hintName;
        foreach (var file in context.AdditionalFiles)
        {
            var (@namespace, name, properties) = Utility.SelectGlobalCompilerVisibleProperty((file, analyzerConfigOptions), cancellationToken);
            if (name is not null && properties.Length > 0)
            {
                var template = new GlobalOptionsTemplate(@namespace ?? options.RootNamespace, name, null, properties);
                template.TransformAppend(builder.Clear());
                text = builder.ToString();
                hintName = builder.Clear().Append(name).Append(".global.cs").ToString();
                context.AddSource(hintName, text);
            }

            (@namespace, name, field, properties) = Utility.SelectAdditionalCompilerVisibleItemMetadata((file, analyzerConfigOptions), cancellationToken);
            if (name is not null && properties.Length > 0)
            {
                var template = new AdditionalFileOptionsTemplate(@namespace ?? options.RootNamespace, name, field, null, properties);
                template.TransformAppend(builder.Clear());
                text = builder.ToString();
                hintName = builder.Clear().Append(name).Append(".additional.cs").ToString();
                context.AddSource(hintName, text);
            }
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();
    }
}
