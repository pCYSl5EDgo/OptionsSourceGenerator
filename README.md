# OptionsSourceGenerator

This is a C# Incremental Source Generator.

# Install

```powershell
dotnet add package OptionsSourceGenerator
```

# Usage

```xml:Example.csproj
<PropertyGroup>
    <RootNamespace>Exam</RootNamespace>
</PropertyGroup>

<ItemGroup>
    <None Include="$(OutputPath)\$(AssemblyName).dll" Pack="true" PackagePath="analyzers/dotnet/roslyn4.0/cs" Visible="false" />
    <None Include="..\..\README.md" Pack="true" PackagePath="\" Visible="false" />

    <Content Include="build\Example.props" Pack="true" PackagePath="build" />
    <Content Include="build\Example.targets" Pack="true" PackagePath="build" />
</ItemGroup>

<ItemGroup>
    <AdditionalFiles Include="\build\Example.props" OptionsSourceGenerator_Namespace="Exam" OptionsSourceGenerator_GlobalName="GlobalOptions" OptionsSourceGenerator_AdditionalFileName="AdditionalFileOptions" />
</ItemGroup>
```
