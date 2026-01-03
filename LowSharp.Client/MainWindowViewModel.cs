using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

using CommunityToolkit.Mvvm.ComponentModel;

using LowSharp.Client.Comon.Views;

namespace LowSharp.Client;

internal sealed class MainWindowViewModel : ObservableObject, IDisposable
{
    public MainWindowViewModel(IDialogs dialogs)
    {
        Client = new ClientViewModel(dialogs);
        Tabs = new BindingList<TabViewModel>();
        CreateTab("Start page", new StartPageViewModel());
    }

    [MemberNotNull(nameof(ActualTabItem))]
    private void CreateTab(string title, ViewModelWithMenus viewModel)
    {
        var newTab = new TabViewModel
        {
            TabTitle = title,
            Content = viewModel
        };
        Tabs.Add(newTab);
        ActualTabItem = newTab;
    }

    public BindingList<TabViewModel> Tabs { get; }

    public TabViewModel ActualTabItem { get; set; }

    public void Dispose()
    {
        Client.Dispose();
    }

    public ClientViewModel Client { get; }
}
