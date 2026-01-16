
namespace LowSharp.ClientLib;

public interface IClient : IDisposable
{
    IExamplesClient Examples { get; }
    IHealtCheckClient HealtCheck { get; }
    bool IsBusy { get; set; }
    bool IsConnected { get; }
    ILoweringClient Lowering { get; }
    IRegexClient Regex { get; }

    event EventHandler? IsBusyChanged;
    event EventHandler? IsConnactedChanged;

    void CloseConnection();
}
