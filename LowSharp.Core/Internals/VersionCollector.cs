namespace LowSharp.Core.Internals;

internal static class VersionCollector
{
    public static IEnumerable<ComponentVersion> GetComponentVersions()
    {
        yield return new ComponentVersion(".NET", Environment.Version);
        yield return new ComponentVersion(nameof(LowSharp.Core), GetVersion(typeof(VersionCollector)));
        yield return new ComponentVersion(nameof(Microsoft.CodeAnalysis.CSharp), GetVersion(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation)));
        yield return new ComponentVersion(nameof(Microsoft.CodeAnalysis.VisualBasic), GetVersion(typeof(Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation)));
        yield return new ComponentVersion(nameof(Microsoft.FSharp), GetVersion(typeof(Microsoft.FSharp.Core.ClassAttribute)));
    }

    public static Version GetVersion(Type t)
    {
        var assembly = t.Assembly;
        var versionInfo = assembly.GetName().Version;
        return versionInfo ?? new Version(0, 0, 0, 0);
    }
}
