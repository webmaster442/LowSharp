using System.Net.WebSockets;

using Grpc.Net.Client;

using LowSharp.ApiV1.Regex;

namespace LowSharp.ClientLib;

internal sealed class RegexClient : IRegexClient
{
    private readonly RegexTester.RegexTesterClient _client;
    private readonly IClientRoot _root;

    public RegexClient(GrpcChannel channel, IClientRoot root)
    {
        _client = new RegexTester.RegexTesterClient(channel);
        _root = root;
    }

    public async Task<Either<RegexMatchResponse, Exception>> MatchAsync(string input,
                                                                        string pattern,
                                                                        RegexOptions options,
                                                                        CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            RegexMatchResponse result = await _client.MatchAsync(new RegexRequest
            {
                Input = input,
                Pattern = pattern,
                Options = options,
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

    public async Task<Either<RegexReplaceResponse, Exception>> ReplaceAsync(string input,
                                                                            string replacement,
                                                                            string pattern,
                                                                            RegexOptions options,
                                                                            CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            RegexReplaceResponse result = await _client.ReplaceAsync(new RegexReplaceRequest
            {
                Input = input,
                Pattern = pattern,
                Replacement = replacement,
                Options = options,
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

    public async Task<Either<RegexSplitResponse, Exception>> SplitAsync(string input,
                                                                        string pattern,
                                                                        RegexOptions options,
                                                                        CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            RegexSplitResponse result = await _client.SplitAsync(new RegexRequest
            {
                Input = input,
                Pattern = pattern,
                Options = options,
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

    public async Task<Either<string, Exception>> GenerateCodeAsync(string input,
                                                                   string pattern,
                                                                   RegexOptions options,
                                                                   CancellationToken cancellation = default)
    {
        try
        {
            _root.IsBusy = true;
            var result = await _client.GenerateCodeAsync(new RegexRequest
            {
                Input = input,
                Pattern = pattern,
                Options = options,
            }, cancellationToken: cancellation).ConfigureAwait(false);

            return result.ResultCode;
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
