using LowSharp.Common.ViewModels;

namespace LowSharp.Common;

internal static class Messages
{
    public sealed record class IsBusyChanged(bool IsBusy);

    public sealed record class IsConnectedChanged(bool IsConnected);

    public sealed record class CloseCurrentTab(ViewModelWithMenus ViewModel);

    public sealed record class ReplaceTabContent(string TabTitle, ViewModelWithMenus ViewModel);

    public sealed record class ClearReplOutput();

    public sealed record class Notification(string Message, TimeSpan Validity);
}
