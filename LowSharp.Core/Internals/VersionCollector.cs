namespace LowSharp.Core.Internals;

internal static class VersionCollector
{
    public static IEnumerable<ComponentVersion> GetComponentVersions()
    {
        yield return new ComponentVersion(".NET", Environment.Version);
        yield return new ComponentVersion("LowSharp", GetVersion(typeof(VersionCollector)));
        yield return new ComponentVersion("Microsoft.CodeAnalysis.CSharp", GetVersion(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation)));
        yield return new ComponentVersion("Microsoft.CodeAnalysis.VisualBasic", GetVersion(typeof(Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)));
        yield return new ComponentVersion("FSharp.Compiler.Service", GetVersion(typeof(FSharp.Compiler.Cancellable)));
    }

    public static Version GetVersion(Type t)
    {
        var assembly = t.Assembly;
        var versionInfo = assembly.GetName().Version;
        return versionInfo ?? new Version(0, 0, 0, 0);
    }
}
