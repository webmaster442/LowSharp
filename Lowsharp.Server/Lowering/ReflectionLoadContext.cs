using System.Reflection;

namespace Lowsharp.Server.Lowering;

internal sealed class ReflectionLoadContext : IDisposable
{
    private readonly MetadataLoadContext _loadContext;
    private bool _disposed;

    public ReflectionLoadContext(ReferenceProvider referenceProvider)
    {
        var resolver = new PathAssemblyResolver(referenceProvider.ReferenceAssemblies);
        _loadContext = new MetadataLoadContext(resolver);
    }

    public void Dispose()
    {
        _loadContext.Dispose();
        _disposed = true;
    }

    public Assembly ConvertToAssembly(Stream stream)
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(ReflectionLoadContext));
        return _loadContext.LoadFromStream(stream);
    }
}
