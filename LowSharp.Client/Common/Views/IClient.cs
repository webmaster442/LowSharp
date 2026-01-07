using LowSharp.ApiV1.Evaluate;
using LowSharp.ApiV1.HealthCheck;
using LowSharp.ApiV1.Lowering;

namespace LowSharp.Client.Common.Views;

internal interface IClient
{
    bool IsBusy { get; }

    Task<bool> DoHealthCheck();

    Task<GetComponentVersionsRespnse> GetComponentVersions();

    Task<LoweringResponse> LowerCodeAsync(string code,
                                     InputLanguage inputLanguage,
                                     Optimization optimization,
                                     OutputCodeType outputCodeType);

    Task<Guid> InitializeReplSession();

    IAsyncEnumerable<FormattedText> SendReplInput(Guid session, string input);
    void HideIsBusy();
}
