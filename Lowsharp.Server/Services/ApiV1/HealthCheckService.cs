using System.Runtime.InteropServices;

using Grpc.Core;

using Lowsharp.Server.Data;

using LowSharp.ApiV1.HealthCheck;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class HealthCheckService : Health.HealthBase
{
    private readonly JsonDbContextCache _jsonDbContextCache;

    public HealthCheckService(JsonDbContextCache jsonDbContextCache)
    {
        _jsonDbContextCache = jsonDbContextCache;
    }

    public override async Task<Empty> InvalidateCache(Empty request, ServerCallContext context)
    {
        await _jsonDbContextCache.InvalidateAsync();
        return new Empty();
    }

    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        var response = new HealthCheckResponse
        {
            Sum = (long)request.Number1 + request.Number2
        };
        return Task.FromResult(response);
    }

    public override async Task<GetComponentVersionsRespnse> GetComponentVersions(Empty request, ServerCallContext context)
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
