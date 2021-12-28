using System.IO;
using System.Xml;

namespace OptionsSourceGenerator;

public static class Utility
{
    private const string Prefix = "build_metadata.AdditionalFiles.OptionsSourceGenerator_";

    public static (string?, string?, ImmutableArray<string>) SelectGlobalCompilerVisibleProperty((AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if ((!pair.Left.Path.EndsWith(".props") && !pair.Left.Path.EndsWith(".targets")))
        {
            return default;
        }

        var options = pair.Right.GetOptions(pair.Left);
        string? @namespace = null;
        if (!options.TryGetValue(Prefix + "GlobalName", out var name))
        {
            name = null;
            if (!options.TryGetValue(Prefix +  "Namespace", out @namespace))
            {
                @namespace = null;
                if (!options.TryGetValue("build_metadata.AdditionalFiles.OptionsSourceGenerator", out _))
                {
                    return default;
                }
            }
        }

        if (pair.Left.GetText(token) is not { Length: > 0 } text)
        {
            return default;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = Path.GetFileNameWithoutExtension(pair.Left.Path);
        }

        return (@namespace, name, SelectCompilerVisiblePropertySortedSet(text.ToString(), token));
    }

    public static ImmutableArray<string> SelectCompilerVisiblePropertySortedSet(string text, CancellationToken token)
    {
        var document = new XmlDocument();
        document.LoadXml(text);
        if (!document.HasChildNodes)
        {
            return ImmutableArray<string>.Empty;
        }

        SortedSet<string> set = new(StringComparer.OrdinalIgnoreCase);
        RecursiveCompilerVisibleProperty(set, document.DocumentElement, token);
        return set.ToImmutableArray();
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

    public static (string?, string?, ImmutableArray<string>) SelectAdditionalCompilerVisibleItemMetadata((AdditionalText Left, AnalyzerConfigOptionsProvider Right) pair, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if ((!pair.Left.Path.EndsWith(".props") && !pair.Left.Path.EndsWith(".targets")))
        {
            return default;
        }

        var options = pair.Right.GetOptions(pair.Left);
        string? @namespace = null;
        if (!options.TryGetValue(Prefix + "AdditionalFileName", out var name))
        {
            name = null;
            if (!options.TryGetValue(Prefix + "Namespace", out @namespace))
            {
                @namespace = null;
                if (!options.TryGetValue("build_metadata.AdditionalFiles.OptionsSourceGenerator", out _))
                {
                    return default;
                }
            }
        }

        if (pair.Left.GetText(token) is not { Length: > 0 } text)
        {
            return default;
        }

        if (string.IsNullOrEmpty(name))
        {
            name = Path.GetFileNameWithoutExtension(pair.Left.Path);
        }

        return (@namespace, name, SelectCompilerVisibleItemMetadataSortedSet(text.ToString(), token));
    }

    private static ImmutableArray<string> SelectCompilerVisibleItemMetadataSortedSet(string text, CancellationToken token)
    {
        var document = new XmlDocument();
        document.LoadXml(text);
        if (!document.HasChildNodes)
        {
            return ImmutableArray<string>.Empty;
        }

        SortedSet<string> set = new(StringComparer.OrdinalIgnoreCase);
        RecursiveCompilerVisibleItemMetadata(set, document.DocumentElement, token);
        return set.ToImmutableArray();
    }

    private static void RecursiveCompilerVisibleItemMetadata(SortedSet<string> set, XmlElement element, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        if (element.Name == "CompilerVisibleItemMetadata")
        {
            var include = element.GetAttribute("Include");
            if (include == "AdditionalFiles")
            {
                var metadataName = element.GetAttribute("MetadataName");
                if (!string.IsNullOrEmpty(metadataName))
                {
                    set.Add(metadataName);
                }
            }

            return;
        }

        var nodes = element.ChildNodes;
        for (int i = 0, count = nodes.Count; i < count; i++)
        {
            var node = nodes[i];
            if (node is XmlElement xmlElement)
            {
                RecursiveCompilerVisibleItemMetadata(set, xmlElement, token);
            }
        }
    }
}
