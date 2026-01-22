

using System.Net.Mime;

using Lowsharp.Server.Infrastructure;

using Microsoft.AspNetCore.StaticFiles;

namespace Lowsharp.Server.Http;

internal sealed class HttpHandler
{
    private readonly WebApplication _application;
    private readonly EmbeddedFiles _embeddedFiles;
    private readonly RequestCache _requestCache;
    private readonly FileExtensionContentTypeProvider _extensionContentTypeProvider;

    public HttpHandler(WebApplication application)
    {
        _application = application;
        _embeddedFiles = application.Services.GetRequiredService<EmbeddedFiles>();
        _requestCache = application.Services.GetRequiredService<RequestCache>();
        _extensionContentTypeProvider = new FileExtensionContentTypeProvider();
        _application.MapGet("/static/graphere.js", async (HttpContext ctx) => await ServeEmbedded("graphere.js", ctx));
        _application.MapGet("/static/nomnoml.js", async (HttpContext ctx) => await ServeEmbedded("nomnoml.js", ctx));
        _application.MapGet("/static/mermaid.min.js", async (HttpContext ctx) => await ServeEmbedded("mermaid.min.js", ctx));

        _application.MapGet("/dynamic/{id}", (Guid id) => ServeDynamicByID(id));
    }

    private IResult ServeDynamicByID(Guid id)
    {
        string? content = _requestCache.GetDynamicHtml(id);
        if (content == null)
        {
            return Results.NotFound();
        }
        return Results.Content(content, MediaTypeNames.Text.Html);
    }

    private async Task ServeEmbedded(string resourceName, HttpContext context)
    {
        if (!_embeddedFiles.CanServe(resourceName))
        {
            await Error(context, StatusCodes.Status404NotFound);
            return;
        }

        using Stream? resourceStream = _embeddedFiles.GetStream(resourceName);
        if (resourceStream == null)
        {
            await Error(context, StatusCodes.Status404NotFound);
            return;
        }

        if (!_extensionContentTypeProvider.TryGetContentType(resourceName, out string? contentType))
        {
            contentType = "application/octet-stream";
        }
        context.Response.ContentType = contentType;
        await resourceStream.CopyToAsync(context.Response.Body);
    }

    private static async Task Error(HttpContext context, int code)
    {
        context.Response.StatusCode = code;
        context.Response.ContentType = "text/plain";
        string txt = code.ToString();
        context.Response.ContentLength = txt.Length;
        await context.Response.WriteAsync(txt);
    }
}
