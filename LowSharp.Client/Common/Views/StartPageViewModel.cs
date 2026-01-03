using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Lowering;

namespace LowSharp.Client.Common.Views;

internal sealed partial class StartPageViewModel : ViewModelWithMenus
{
    private readonly IClient _client;
    private readonly IDialogs _dialogs;

    public StartPageViewModel(IClient client, IDialogs dialogs)
    {
        _client = client;
        _dialogs = dialogs;
    }

    [RelayCommand]
    public void StartLowering()
        => ReplaceContents("Lowering", new LoweringViewModel(_client, _dialogs));

    [RelayCommand]
    public void StartRepl()
    {

    }
}
