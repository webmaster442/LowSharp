
using LowSharp.ApiV1.HealthCheck;

namespace LowSharp.ClientLib;

public interface IHealtCheckClient
{
    Task<Either<bool, Exception>> DoHealthCheckAsync(CancellationToken cancellation = default);
    Task<Either<GetComponentVersionsRespnse, Exception>> GetComponentVersionsAsync(CancellationToken cancellation = default);
    Task<Either<bool, Exception>> InvalidateCacheAsync(CancellationToken cancellation = default);
}
