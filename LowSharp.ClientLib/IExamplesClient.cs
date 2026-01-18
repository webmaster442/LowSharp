
using LowSharp.ApiV1.Examples;

namespace LowSharp.ClientLib;

public interface IExamplesClient
{
    Task<Either<List<Example>, Exception>> GetExamplesAsync(CancellationToken cancellation = default);
}
