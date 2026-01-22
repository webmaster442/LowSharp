using LowSharp.ApiV1.Lowering;

namespace LowSharp.ClientLib;

public interface ILoweringClient
{
    IClientCommon Common { get; }

    Task<Either<LoweringResponse, Exception>> LowerCodeAsync(string code,
                                                             InputLanguage inputLanguage,
                                                             Optimization optimization,
                                                             OutputCodeType outputCodeType,
                                                             CancellationToken cancellation = default);
    Task<Either<Uri, Exception>> RenderVisualizationAsync(string code,
                                                          VisualType visualType,
                                                          CancellationToken cancellation = default);
}
