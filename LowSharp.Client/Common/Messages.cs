using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Common;

internal static class Messages
{
    public sealed record class IsBusyChangedMessage(bool IsBusy);

    public sealed record class IsConnectedChangedMessage(bool IsConnected);

    public sealed record class CloseCurrentTabMessage(ViewModelWithMenus ViewModel);

    public sealed record class ReplaceTabContentMessage(string tabTitle, ViewModelWithMenus ViewModel);

    public sealed record class SetInputCodeRequest(string Code);

    public sealed record class SetOutputCodeRequest(string Code);
}
