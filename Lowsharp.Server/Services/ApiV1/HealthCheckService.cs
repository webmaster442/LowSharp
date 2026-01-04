using Grpc.Core;

using LowSharp.ApiV1.HealthCheck;

namespace Lowsharp.Server.Services.ApiV1;

internal sealed class HealthCheckService : Health.HealthBase
{
    public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
    {
        var response = new HealthCheckResponse
        {
            Sum = (long)request.Number1 + request.Number2
        };
        return Task.FromResult(response);
    }
}
