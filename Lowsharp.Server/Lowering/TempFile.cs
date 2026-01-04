using System.Text;

namespace Lowsharp.Server.Lowering;

internal sealed class TempFile : IDisposable
{
    private bool _disposed;

    public string FullPath { get; }

    public bool IsExisting
        => File.Exists(FullPath);

    public TempFile(string extension)
    {
        FullPath = Path.Combine(Path.GetTempPath(), Path.ChangeExtension(Path.GetRandomFileName(), extension));
    }

    public async Task WriteAsync(string content, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await File.WriteAllTextAsync(FullPath, content, Encoding.UTF8, cancellationToken);
    }

    public async Task CopyTo(Stream target, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await using var stream = File.OpenRead(FullPath);
        await stream.CopyToAsync(target, cancellationToken);
    }

    public override string ToString()
        => FullPath;

    public void Dispose()
    {
        if (File.Exists(FullPath))
        {
            try
            {
                File.Delete(FullPath);
            }
            catch { }
        }
        _disposed = true;
    }
}
