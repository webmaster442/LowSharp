

namespace LowSharp.ClientLib;

public interface IClient : IDisposable, IClientCommon
{
    IExamplesClient Examples { get; }
    IHealtCheckClient HealtCheck { get; }
    bool IsBusy { get; set; }
    bool IsConnected { get; }
    ILoweringClient Lowering { get; }
    IRegexClient Regex { get; }

    event EventHandler? IsBusyChanged;
    event EventHandler? IsConnectedChanged;

    void CloseConnection();
    Task<Either<bool, Exception>> Connect(string serverUriWithoutPort, int gcpPort, int httpPort);
}
