using Grpc.Core;

using Lowsharp.Server.Infrastructure;
using Lowsharp.Server.Lowering;
using Lowsharp.Server.Visualization;

using LowSharp.ApiV1.Lowering;

using Microsoft.CodeAnalysis.Scripting;

namespace Lowsharp.Server.Services.ApiV1;

internal class LowererService : Lowerer.LowererBase
{
    private readonly LoweringEngine _engine;
    private readonly RazorViewRenderer _renderer;
    private readonly RequestCache _cache;
    private readonly ILogger _logger;

    public LowererService(LoweringEngine engine,
                          RazorViewRenderer renderer,
                          RequestCache cache,
                          ILoggerFactory loggerFactory)
    {
        _engine = engine;
        _renderer = renderer;
        _cache = cache;
        _logger = loggerFactory.CreateLogger<LowererService>();
    }

    public override async Task<LoweringResponse> ToLowerCode(LoweringRequest request, ServerCallContext context)
    {
        EngineInput engineInput = Mapper.Map(request);

        return await _cache.GetOrCreateAsync(engineInput, async input =>
        {
            _logger.LogInformation("Lowering code for request: {Request}", input);
            EngineOutput output = await _engine.ToLowerCodeAsync(input, context.CancellationToken);
            return Mapper.Map(output);
        });
    }

    public override async Task<RenderVisualizationResponse> RenderVisualization(RenderVisualizationRequest request, ServerCallContext context)
    {
        var result = await _renderer.Render<Nomnoml>(new Dictionary<string, object?>()
        {
            { "GraphereUrl", $"{request.ServerUrl}static/graphere.js" },
            { "NomnomlUrl", $"{request.ServerUrl}static/nomnoml.js" },
            { "Code", request.InputCode }
        });

        return new RenderVisualizationResponse
        {
            Html = result
        };
    }
}
