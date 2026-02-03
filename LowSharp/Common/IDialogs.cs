

using LowSharp.ApiV1.Lowering;

namespace LowSharp.Common;

internal interface IDialogs
{
    void ClientError(Exception failure);
    bool Confirm(string title, string message);
    void Error(string title, string message);
    void Info(string title, string message);
    void OpenUrl(string url);
    void OpenWebView(string title, Uri url);
    bool TryOpenCode(out (string filename, InputLanguage language) result);
    bool TryOpen(string title, string filter, out string filename);
    bool TrySave(string title, string filter, out string filename);
}
