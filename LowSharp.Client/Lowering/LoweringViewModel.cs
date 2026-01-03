using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client.Lowering;

internal sealed partial class LoweringViewModel :
    ViewModelWithMenus,
    IRecipient<Messages.IsBusyChangedMessage>
{
    private readonly IClient _client;

    [ObservableProperty]
    public partial bool IsBusy { get; set; }

    public MenuCheckableViewModel ShowLineNumbers { get; }

    public MenuCheckableViewModel WordWrap { get; }

    public LoweringViewModel(IClient client)
    {
        _client = client;
        IsBusy = client.IsBusy;
        WeakReferenceMessenger.Default.Register<Messages.IsBusyChangedMessage>(this);

        ShowLineNumbers = new MenuCheckableViewModel
        {
            Header = "Show Line Numbers",
            IsChecked = true
        };

        WordWrap = new MenuCheckableViewModel
        {
            Header = "Word Wrap",
            IsChecked = true
        };

        Menus.Add(new MenuViewModel
        {
            Header = "View",
            Children =
            {
                ShowLineNumbers,
                WordWrap
            }
        });
    }

    void IRecipient<Messages.IsBusyChangedMessage>.Receive(Messages.IsBusyChangedMessage message)
        => IsBusy = message.IsBusy;
}
