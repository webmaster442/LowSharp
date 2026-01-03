using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Common;

internal static class Messages
{
    public sealed record class IsBusyChanged(bool IsBusy);

    public sealed record class IsConnectedChanged(bool IsConnected);

    public sealed record class CloseCurrentTab(ViewModelWithMenus ViewModel);

    public sealed record class ReplaceTabContent(string tabTitle, ViewModelWithMenus ViewModel);

    public sealed record class SetInputLanguage(string Language);

    public sealed record class SetInputCode(string Code);

    public sealed record class SetOutputCodeRequest(string Code);

    public sealed record class TabIndexChanged(int NewIndex);
}
