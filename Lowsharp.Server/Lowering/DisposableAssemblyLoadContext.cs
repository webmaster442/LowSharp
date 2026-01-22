using System.Runtime.Loader;

namespace Lowsharp.Server.Lowering;

internal sealed class DisposableAssemblyLoadContext : AssemblyLoadContext, IDisposable
{
    public DisposableAssemblyLoadContext() : base(isCollectible: true)
    {
    }

    public void Dispose()
    {
        Unload();
    }
}
