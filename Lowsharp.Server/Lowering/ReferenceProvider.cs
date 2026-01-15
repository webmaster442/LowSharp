using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Lowsharp.Server.Lowering;

internal sealed class ReferenceProvider
{
    public ReferenceProvider()
    {
        var systemAssemblyFolder = Path.GetDirectoryName(typeof(Object).Assembly.Location);

        if (systemAssemblyFolder == null)
            throw new InvalidOperationException("Couldn't determine system assembly folder");

        var sdkVersionString = Path.GetFileName(systemAssemblyFolder);

        if (!Version.TryParse(sdkVersionString, out Version? parsedVersion))
            throw new InvalidOperationException("Couldn't determine sdk version");

        string dontetVersion = $"net{parsedVersion.Major}.{parsedVersion.Minor}";

        var dotnetRoot = Path.GetFullPath(Path.Combine(systemAssemblyFolder, "..", "..", ".."));

        var referencePaths = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref", sdkVersionString, "ref", dontetVersion);

        if (!Directory.Exists(referencePaths))
            throw new InvalidOperationException("Couldn't determine reference assembly folder");

        ReferenceAssemblies = Directory.GetFiles(referencePaths, "*.dll");

        var analyzerPath = Path.Combine(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref", sdkVersionString, "analyzers", "dotnet", "cs");

        if (Directory.Exists(analyzerPath))
        {
            AnalyzerAssemblies = Directory.GetFiles(analyzerPath, "*.dll");
        }
        else
        {
            AnalyzerAssemblies = Array.Empty<string>();
        }

        MetadataReferences = GetMetadataReferences().ToArray();
        SourceGeneratgors = GetSourceGenerators();
    }

    public string[] ReferenceAssemblies { get; }

    public string[] AnalyzerAssemblies { get; }

    public MetadataReference[] MetadataReferences { get; }

    public IReadOnlyList<IIncrementalGenerator> SourceGeneratgors { get; }

    public IEnumerable<MetadataReference> GetMetadataReferences()
    {
        return ReferenceAssemblies
                .Select(p => MetadataReference.CreateFromFile(p));
    }

    public IReadOnlyList<IIncrementalGenerator> GetSourceGenerators()
    {
        List<IIncrementalGenerator> result = new();
        foreach (var analyzerPath in AnalyzerAssemblies)
        {
            var assembly = Assembly.LoadFrom(analyzerPath);
            var generatorTypes = assembly.GetTypes()
                    .Where(t => typeof(IIncrementalGenerator).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic);
            foreach (var generatorType in generatorTypes)
            {
                if (Activator.CreateInstance(generatorType) is IIncrementalGenerator generator)
                {
                    result.Add(generator);
                }
            }
        }
        return result;
    }
}
