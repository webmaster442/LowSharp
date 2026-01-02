using CommunityToolkit.Mvvm.ComponentModel;

using LowSharp.Client.Comon.Views;

namespace LowSharp.Client;

internal sealed class MainWindowViewModel : ObservableObject, IDisposable
{
    public MainWindowViewModel(IDialogs dialogs)
    {
        Client = new ClientViewModel(dialogs);
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    public ClientViewModel Client { get; }
}
