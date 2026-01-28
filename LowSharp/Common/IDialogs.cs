

namespace LowSharp.Common;

internal interface IDialogs
{
    void ClientError(Exception failure);
    bool Confirm(string title, string message);
    void Error(string title, string message);
    void Info(string title, string message);
    void OpenUrl(string url);
}
