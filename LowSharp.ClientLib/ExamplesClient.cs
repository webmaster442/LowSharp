using Grpc.Core;
using Grpc.Net.Client;
using LowSharp.ApiV1.Examples;

namespace LowSharp.ClientLib;

internal sealed class ExamplesClient : IExamplesClient
{
    private readonly Examples.ExamplesClient _client;
    private readonly IClientRoot _root;

    public ExamplesClient(GrpcChannel channel, IClientRoot root)
    {
        _client = new Examples.ExamplesClient(channel);
        _root = root;
    }

    public async Task<Either<List<Example>, Exception>> GetExamplesAsync(CancellationToken cancellation = default)
    {
        _root.ThrowIfCantContinue();
        try
        {
            _root.IsBusy = true;
            List<Example> results = new();
            AsyncServerStreamingCall<Example> stream = _client.GetExamples(new GetExamplesRequest());
            return await stream.ResponseStream.ReadAllAsync(cancellation).ToListAsync(cancellation);
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
