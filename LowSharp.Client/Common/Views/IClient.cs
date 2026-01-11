using LowSharp.ApiV1.Evaluate;
using LowSharp.ApiV1.HealthCheck;
using LowSharp.ApiV1.Lowering;
using LowSharp.ApiV1.Regex;

namespace LowSharp.Client.Common.Views;

internal interface IClient
{
    bool IsBusy { get; }

    void HideIsBusy();

    Task<bool> DoHealthCheckAsync();

    Task<GetComponentVersionsRespnse> GetComponentVersionsAsync();

    Task<LoweringResponse> LowerCodeAsync(string code,
                                     InputLanguage inputLanguage,
                                     Optimization optimization,
                                     OutputCodeType outputCodeType);

    Task<Guid> InitializeReplSessionAsync();

    IAsyncEnumerable<FormattedText> SendReplInputAsync(Guid session, string input);

    Task<string> RegexReplaceAsync(string input,
                                   string replacement,
                                   string pattern,
                                   RegexOptions options);

    Task<RegexMatchResponse> RegexMatchAsync(string input,
                                             string pattern,
                                             RegexOptions options);

    Task<IList<string>> RegexSplitAsync(string input,
                                        string pattern,
                                        RegexOptions options);
}
