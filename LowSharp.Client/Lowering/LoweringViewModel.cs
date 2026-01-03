using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Comon;
using LowSharp.Client.Comon.Views;

namespace LowSharp.Client.Lowering;

internal sealed partial class LoweringViewModel :
    ViewModelWithMenus,
    IRecipient<Messages.IsBusyChangedMessage>
{
    private readonly IClient _client;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public LoweringViewModel(IClient client)
    {
        _client = client;
        IsBusy = client.IsBusy;
        WeakReferenceMessenger.Default.Register<Messages.IsBusyChangedMessage>(this);
    }

    void IRecipient<Messages.IsBusyChangedMessage>.Receive(Messages.IsBusyChangedMessage message)
        => IsBusy = message.IsBusy;
}
