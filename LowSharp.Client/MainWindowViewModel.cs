using CommunityToolkit.Mvvm.ComponentModel;

using LowSharp.Client.Comon;

namespace LowSharp.Client;

internal sealed class MainWindowViewModel : ObservableObject
{
    public MainWindowViewModel()
    {
        Client = new ClientViewModel();
    }

    public ClientViewModel Client { get; }
}
