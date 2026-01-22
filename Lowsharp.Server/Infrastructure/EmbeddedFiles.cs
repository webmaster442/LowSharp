namespace Lowsharp.Server.Infrastructure;

internal sealed class EmbeddedFiles
{
    private readonly Dictionary<string, string> _files;

    public EmbeddedFiles()
    {
         _files = typeof(EmbeddedFiles).Assembly
            .GetManifestResourceNames()
            .ToDictionary(f => GetEmbeddedFileName(f), f => f);
    }

    public bool CanServe(string fileName)
    {
        return _files.ContainsKey(fileName);
    }

    public Stream? GetStream(string fileName)
    {
        if (_files.TryGetValue(fileName, out string? value))
        {
            return typeof(EmbeddedFiles).Assembly.GetManifestResourceStream(value);
        }
        return null;
    }

    public static string GetEmbeddedFileName(string embeddedName)
    {
        var parts = embeddedName.Split('.', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length == 0)
            return string.Empty;

        return $"{parts[^2]}.{parts[^1]}";
    }
}
