using LowSharp.Lowering.ApiV1;

namespace LowSharp.Client.Common.Views;

internal interface IDialogs
{
    Task Error(string title, string message);
    bool TryOpenCode(out (string filename, InputLanguage language) result);
}
