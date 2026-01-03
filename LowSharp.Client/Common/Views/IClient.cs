using LowSharp.Lowering.ApiV1;

namespace LowSharp.Client.Common.Views;

internal interface IClient
{
    bool IsBusy { get; }

    Task<LoweringResponse> LowerCodeAsync(string code,
                                     InputLanguage inputLanguage,
                                     Optimization optimization,
                                     OutputCodeType outputCodeType);
}
