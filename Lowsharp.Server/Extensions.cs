using Microsoft.AspNetCore.StaticFiles;
using Microsoft.CodeAnalysis;

namespace Lowsharp.Server;

internal static class Extensions
{
   private static Stream GetFile(string resourceName)
    {
        var assembly = typeof(Extensions).Assembly;
        var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException($"Embedded resource '{resourceName}' not found.");
        }
        return stream;
    }

    private static string GetMimeType(FileExtensionContentTypeProvider contentTypeProvider, string resourceName)
    {
        var extension = Path.GetExtension(resourceName).ToLowerInvariant();
        return contentTypeProvider.TryGetContentType(extension, out var contentType)
            ? contentType
            : "application/octet-stream";
    }

    public static void MapEmbeddedFile(
        this IEndpointRouteBuilder endpoints,
        string pattern,
        string resourceName,
        FileExtensionContentTypeProvider contentTypeProvider)
    {
        endpoints.MapGet(pattern, async context =>
        {
            using var stream = GetFile(resourceName);
            var mimeType = GetMimeType(contentTypeProvider, resourceName);
            context.Response.ContentType = mimeType;
            await stream.CopyToAsync(context.Response.Body);
        });
    }
}
