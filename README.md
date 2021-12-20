# OptionsSourceGenerator

This is a C# Incremental Source Generator.

# Install

```powershell
dotnet add package OptionsSourceGenerator
```

# Usage

```xml:Example.csproj
<ItemGroup>
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/roslyn4.0/cs" Visible="false" />
    <None Include="..\..\README.md" Pack="true" PackagePath="\" Visible="false" />

    <Content Include="build\OptionsSourceGenerator.props" Pack="true" PackagePath="build" />
    <Content Include="build\OptionsSourceGenerator.targets" Pack="true" PackagePath="build" />
</ItemGroup>

<ItemGroup>
    <AdditionalFiles Include="\build\import.props" OptionsSourceGenerator="" />
</ItemGroup>
```
