using Grpc.Net.Client;

using LowSharp.ApiV1.Lowering;

namespace LowSharp.ClientLib;

internal sealed class LoweringClient : ILoweringClient
{
    private readonly Lowerer.LowererClient _client;
    private readonly IClientRoot _root;

    public LoweringClient(GrpcChannel channel, IClientRoot root)
    {
        _client = new Lowerer.LowererClient(channel);
        _root = root;
    }

    public async Task<Either<string, Exception>> RenderVisualizationAsync(string code,
                                                                          VisualType visualType,
                                                                          CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            var result = await _client.RenderVisualizationAsync(new RenderVisualizationRequest
            {
                InputCode = code,
                ServerUrl = _root.HttpUrl,
                VisualType = visualType,
            });
            return result.Html;
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

    public async Task<Either<LoweringResponse, Exception>> LowerCodeAsync(string code,
                                                                          InputLanguage inputLanguage,
                                                                          Optimization optimization,
                                                                          OutputCodeType outputCodeType,
                                                                          CancellationToken cancellation = default)
    {
        _root.ThrowIfCantContinue();
        try
        {
            _root.IsBusy = true;

            var result = await _client.ToLowerCodeAsync(new LoweringRequest()
            {
                Code = code,
                Language = inputLanguage,
                OptimizationLevel = optimization,
                OutputType = outputCodeType
            }, cancellationToken: cancellation).ConfigureAwait(false);

            return result;
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
