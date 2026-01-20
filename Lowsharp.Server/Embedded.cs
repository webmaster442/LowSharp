
namespace Lowsharp.Server;

internal static class Embedded
{
    private static readonly string[] _embeddedFiles
        = typeof(Embedded).Assembly.GetManifestResourceNames();

    internal static string? GetAsString(string file)
    {
        var fullName = _embeddedFiles
            .FirstOrDefault(name => name.EndsWith(file, StringComparison.OrdinalIgnoreCase));

        if (fullName == null)
            throw new InvalidOperationException($"Embedded resource '{file}' not found.");
        
        using var stream = typeof(Embedded).Assembly.GetManifestResourceStream(fullName);
        using var reader = new StreamReader(stream!);

        return reader.ReadToEnd();
    }
}
