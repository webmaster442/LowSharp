using Grpc.Core;

using Lowsharp.Server.Infrastructure;
using Lowsharp.Server.Lowering;

using LowSharp.ApiV1.Lowering;

using Microsoft.Extensions.Caching.Memory;

namespace Lowsharp.Server.Services.ApiV1;

internal class LowererService : Lowerer.LowererBase
{
    private readonly LoweringEngine _engine;
    private readonly RequestCache _cache;
    private readonly ILogger _logger;

    public LowererService(LoweringEngine engine, RequestCache cache, ILoggerFactory loggerFactory)
    {
        _engine = engine;
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
}
