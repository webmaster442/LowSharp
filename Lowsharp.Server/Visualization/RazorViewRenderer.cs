using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.HtmlRendering;

namespace Lowsharp.Server.Visualization;

internal sealed class RazorViewRenderer : IDisposable
{
    private readonly HtmlRenderer _htmlRenderer;
    private bool _disposed;

    public RazorViewRenderer(IServiceProvider services, ILoggerFactory loggerFactory)
    {
        _htmlRenderer = new HtmlRenderer(services, loggerFactory);
    }

    public void Dispose()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _htmlRenderer.Dispose();
        _disposed = true;
    }

    public async Task<string> Render<TComponent>(IDictionary<string, object?>? parameters = null)
        where TComponent : IComponent
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        string html = await _htmlRenderer.Dispatcher.InvokeAsync(async () =>
        {
            ParameterView mappedParams = parameters == null
                ? ParameterView.Empty
                : ParameterView.FromDictionary(parameters);

            HtmlRootComponent output = await _htmlRenderer.RenderComponentAsync<TComponent>(mappedParams);

            return output.ToHtmlString();
        });

        return html;
    }
}
