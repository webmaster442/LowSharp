

namespace LowSharp.ClientLib;

public interface IClient : IDisposable, IClientCommon
{
    IExamplesClient Examples { get; }
    IHealthCheckClient HealtCheck { get; }
    ILoweringClient Lowering { get; }
    IRegexClient Regex { get; }
    ISchemaToCodeClient SchemaToCode { get; }

    bool IsBusy { get; set; }
    bool IsConnected { get; }
    event EventHandler? IsBusyChanged;
    event EventHandler? IsConnectedChanged;

    void CloseConnection();
    Task<Either<bool, Exception>> Connect(string serverUriWithoutPort, int gcpPort, int httpPort);
}
