using System.Windows;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;
using LowSharp.Client.Lowering;

namespace LowSharp.Client.Common.Views;

internal sealed partial class StartPageViewModel : ViewModelWithMenus
{
    [RelayCommand]
    public void StartLowering()
    {
        var message = WeakReferenceMessenger.Default.Send<Messages.RequestClientMessage>();
        ReplaceContents("Lowering", new LoweringViewModel(message.Response));
    }

    [RelayCommand]
    public void StartRepl()
    {

    }
}
