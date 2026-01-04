using System.Runtime.InteropServices;

using Grpc.Core;

using Lowsharp.Server.Data;
using Lowsharp.Server.Lowering;

using LowSharp.ApiV1.Lowering;

namespace Lowsharp.Server.Services.ApiV1;

internal class LowererService : Lowerer.LowererBase
{
    private readonly LoweringEngine _engine;
    private readonly JsonDbContextCache _cache;
    private readonly ILogger _logger;

    public LowererService(LoweringEngine engine, JsonDbContextCache cache, ILoggerFactory loggerFactory)
    {
        _engine = engine;
        _cache = cache;
        _logger = loggerFactory.CreateLogger<LowererService>();
    }

    public override async Task<LoweringResponse> ToLowerCode(LoweringRequest request, ServerCallContext context)
    {
        EngineInput engineInput = Mapper.Map(request);

        return await _cache.TryGetOrCreate(engineInput, async input =>
        {
            _logger.LogInformation("Lowering code for request: {Request}", input);
            EngineOutput output = await _engine.ToLowerCodeAsync(input, context.CancellationToken);
            return Mapper.Map(output);
        });
    }

    public override async Task<GetComponentVersionsRespnse> GetComponentVersions(GetComponentVersionsRequest request, ServerCallContext context)
    {
        var response = new GetComponentVersionsRespnse
        {
            OperatingSystem = RuntimeInformation.OSDescription,
            OperatingSystemVersion = Environment.OSVersion.Version.ToString(),
            RuntimeVersion = RuntimeInformation.FrameworkDescription,
        };

        IOrderedEnumerable<(string name, string version)> versions
            = VersionCollector.LoadedAsssemblyVersions().OrderBy(x => x.name);

        foreach (var (name, version) in versions)
        {
            response.ComponentVersions.Add(new ComponentVersion
            {
                Name = name,
                VersionString = version,
            });
        }


        return response;
    }
}
