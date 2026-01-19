using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Lowsharp.Server.Visualization;

internal sealed class RazorViewRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorViewRenderer(IRazorViewEngine viewEngine,
                    ITempDataProvider tempDataProvider,
                    IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> Render<TModel>(string viewName, TModel model)
    {
        var httpContext = new DefaultHttpContext
        {
            RequestServices = _serviceProvider
        };

        var actionContext = new ActionContext(httpContext,
                                              new RouteData(),
                                              new ActionDescriptor());

        using var sw = new StringWriter();

        ViewEngineResult viewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: false);
        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"View '{viewName}' not found.");
        }

        var viewDictionary = new ViewDataDictionary<TModel>(new EmptyModelMetadataProvider(),
                                                            new ModelStateDictionary())
        {
            Model = model
        };

        var viewContext = new ViewContext(actionContext,
                                          viewResult.View,
                                          viewDictionary,
                                          new TempDataDictionary(httpContext, _tempDataProvider),
                                          sw,
                                          new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);

        return sw.ToString();
    }
}
