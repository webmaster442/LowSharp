using Grpc.Net.Client;

using LowSharp.ApiV1.HealthCheck;

namespace LowSharp.ClientLib;

internal sealed class HealtCheckClient : IHealtCheckClient
{
    private readonly Health.HealthClient _client;
    private readonly IClientRoot _root;

    internal HealtCheckClient(GrpcChannel channel, IClientRoot root)
    {
        _client = new Health.HealthClient(channel);
        _root = root;
    }

    public async Task<Either<bool, Exception>> DoHealthCheckAsync(CancellationToken cancellation = default)
    {
        _root.ThrowIfCantContinue();
        try
        {
            _root.IsBusy = true;
            int number1 = Random.Shared.Next();
            int number2 = Random.Shared.Next();
            long expectedSum = (long)number1 + (long)number2;
            var response = await _client.CheckAsync(new HealthCheckRequest
            {
                Number1 = number1,
                Number2 = number2,
            }, cancellationToken: cancellation).ConfigureAwait(false);

            return response.Sum == expectedSum;
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            _root.IsBusy = false;
        }
    }

    public async Task<Either<bool, Exception>> InvalidateCacheAsync(CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;

            await _client.InvalidateCacheAsync(new Empty(), cancellationToken: cancellation)
                .ConfigureAwait(false);

            return true;
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            _root.IsBusy = false;
        }
    }

    public async Task<Either<ComponentVersion[], Exception>> GetComponentVersionsAsync(CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            var response = await _client.GetComponentVersionsAsync(new Empty(), cancellationToken: cancellation)
                .ConfigureAwait(false);

            return response.ComponentVersions.ToArray();
        }
        catch (Exception ex)
        {
            return ex;
        }
        finally
        {
            _root.IsBusy = false;
        }
    }
}
