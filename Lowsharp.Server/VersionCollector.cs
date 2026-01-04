using System.Reflection;
using System.Runtime.Loader;

namespace Lowsharp.Server;

internal static class VersionCollector
{
    public static IEnumerable<(string name, string version)> LoadedAsssemblyVersions()
    {
        foreach (var assembly in AssemblyLoadContext.Default.Assemblies)
        {
            AssemblyName assemblyName = assembly.GetName();

            var name = assemblyName.Name ?? "Unknown";
            var version = assemblyName.Version?.ToString() ?? "Unknown Version";

            yield return (name, version);
        }
    }
}
