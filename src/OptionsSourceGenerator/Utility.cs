using System.Text;
using System.Xml;

namespace OptionsSourceGenerator;

public static class Utility
{
    public static IEnumerable<string> SelectCompilerVisibleProperty((AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (pair.Left.Path.EndsWith(".props")
        || pair.Left.Path.EndsWith(".targets")
        || !pair.Right.GetOptions(pair.Left).TryGetValue("build_metadata.AdditionalFiles.OptionsSourceGenerator", out _))
        {
            return Array.Empty<string>();
        }

        if (pair.Left.GetText(token) is not { Length: > 0 } text)
        {
            return Array.Empty<string>();
        }

        var document = new XmlDocument();
        document.LoadXml(text.ToString());
        if (!document.HasChildNodes)
        {
            return Array.Empty<string>();
        }

        SortedSet<string> set = new(StringComparer.OrdinalIgnoreCase);
        RecursiveCompilerVisibleProperty(set, document.DocumentElement, token);
        return set;
    }

    private static void RecursiveCompilerVisibleProperty(SortedSet<string> set, XmlElement element, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (element.Name == "CompilerVisibleProperty")
        {
            var include = element.GetAttribute("Include");
            if (!string.IsNullOrEmpty(include))
            {
                set.Add(include);
            }

            var exclude = element.GetAttribute("Exclude");
            if (!string.IsNullOrEmpty(exclude))
            {
                set.Remove(exclude);
            }

            return;
        }

        var nodes = element.ChildNodes;
        for (int i = 0, count = nodes.Count; i < count; i++)
        {
            var node = nodes[i];
            if (node is XmlElement xmlElement)
            {
                RecursiveCompilerVisibleProperty(set, xmlElement, token);
            }
        }
    }

    public static string GenerateSource(Options options, System.Collections.Immutable.ImmutableArray<string> properties, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        StringBuilder builder = new();
        builder.Append("namespace ")
            .AppendLine(string.IsNullOrWhiteSpace(options.RootNamespace) ? "OptionsSourceGenerator" : options.RootNamespace)
            .AppendLine("{")
            .AppendLine("    public sealed partial class Options : global::System.IEquatable<Options>")
            .AppendLine("    {");

        foreach (var property in properties)
        {
            token.ThrowIfCancellationRequested();
            builder.Append("        public readonly string? ").Append(property).AppendLine(";");
        }

        builder.AppendLine()
            .AppendLine("        public Options(global::Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptions options)")
            .AppendLine("        {");

        foreach (var property in properties)
        {
            token.ThrowIfCancellationRequested();
            builder
                .Append("            if (!options.TryGetValue(\"build_property.").Append(property).Append("\", out ").Append(property).AppendLine("))")
                .AppendLine("            {")
                .Append("                ").Append(property).AppendLine(" = null;")
                .AppendLine("            }");

        }

        builder.AppendLine("        }")
            .AppendLine()
            .Append("        public bool Equals(Options other) => ");

        if (properties.Length > 0)
        {
            var property = properties[0];
            builder.Append(property).Append(" == other.").Append(property);

            for (int i = 1; i < properties.Length; i++)
            {
                token.ThrowIfCancellationRequested();
                property = properties[i];
                builder.Append(" && ").Append(property).Append(" == other.").Append(property);
            }

            builder.AppendLine(";");
        }
        else
        {
            builder.AppendLine("true;");
        }

        token.ThrowIfCancellationRequested();
        return builder
            .AppendLine("        public static Options Select(global::Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider provider, global::System.Threading.CancellationToken token)")
            .AppendLine("        {")
            .AppendLine("            token.ThrowIfCancellationRequested();")
            .AppendLine("            return new Options(provider.GlobalOptions);")
            .AppendLine("        }")
            .AppendLine("    }")
            .AppendLine("}")
            .AppendLine()
            .ToString();
    }
}
