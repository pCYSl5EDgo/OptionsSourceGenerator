using Xunit;

namespace OptionsSourceGenerator.Test;

public class Test
{
    const string source = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Project>
	<ItemGroup>
		<CompilerVisibleProperty Include=""DesignTimeBuild"" />
		<CompilerVisibleProperty Include=""RootNamespace"" />

		<CompilerVisibleItemMetadata Include=""AdditionalFiles"" MetadataName=""OptionsSourceGenerator"" />
	</ItemGroup>
</Project>";

    [Fact]
    public void EnumerationTest()
    {
        var set = Utility.SelectCompilerVisiblePropertySortedSet(source, default);
        var enumerator = set.GetEnumerator();
        Assert.True(enumerator.MoveNext());
        Assert.Equal("DesignTimeBuild", enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal("RootNamespace", enumerator.Current);
        Assert.False(enumerator.MoveNext());
    }
}
