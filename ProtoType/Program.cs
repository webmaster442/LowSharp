var provider = new ReferenceProvider();

public class ReferenceProvider
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
    }

    public string[] ReferenceAssemblies { get; }

    public string[] AnalyzerAssemblies { get; }
}


