using CommunityToolkit.Mvvm.Messaging.Messages;

using LowSharp.Client.Comon.Views;

namespace LowSharp.Client.Comon;

internal static class Messages
{
    public sealed record class IsBusyChangedMessage(bool IsBusy);

    public sealed record class IsConnectedChangedMessage(bool IsConnected);

    public sealed class RequestClientMessage : RequestMessage<IClient>;

    public sealed record class CloseCurrentTabMessage(ViewModelWithMenus ViewModel);

    public sealed record class ReplaceTabContentMessage(string tabTitle, ViewModelWithMenus ViewModel);
}
