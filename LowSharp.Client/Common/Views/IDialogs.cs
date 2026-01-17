
using LowSharp.ApiV1.Lowering;

namespace LowSharp.Client.Common.Views;

internal interface IDialogs
{
    Task Error(string title, string message);
    Task Info(string title, string message);
    Task<bool> Confirm(string title, string message);
    bool TryOpenCode(out (string filename, InputLanguage language) result);
    void Notify(string message, int validityInSeconds);
    Task ClientError(Exception failure);
}
