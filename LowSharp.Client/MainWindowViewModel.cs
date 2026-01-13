using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;
using LowSharp.Client.Common.Views;

namespace LowSharp.Client;

internal sealed partial class MainWindowViewModel :
    ObservableObject,
    IDisposable,
    IRecipient<Messages.ReplaceTabContent>,
    IRecipient<Messages.CloseCurrentTab>,
    IRecipient<Messages.IsConnectedChanged>
{
    private const int BaseFontSize = 16;
    private readonly IDialogs _dialogs;

    public MainWindowViewModel(IDialogs dialogs)
    {
        _dialogs = dialogs;
        Client = new ClientViewModel(dialogs);
        Notifications = new NotificationsViewModel();
        Tabs = new BindingList<TabViewModel>();
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        ActualZoomLevel = 1.0;
        CreateStartPage();
        WeakReferenceMessenger.Default.Register<Messages.ReplaceTabContent>(this);
        WeakReferenceMessenger.Default.Register<Messages.CloseCurrentTab>(this);
        WeakReferenceMessenger.Default.Register<Messages.IsConnectedChanged>(this);

        _dialogs.Notify("Welcome to LowSharp! To Connect to a server click on the 'Disconnected item' on the left.", 20);
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

    [MemberNotNull(nameof(ActualTabItem))]
    private void CreateStartPage()
        => CreateTab("Start page", new StartPageViewModel(Client.ApiClient, _dialogs));

    public void Dispose()
    {
        Client.Dispose();
    }

    public ClientViewModel Client { get; }

    public NotificationsViewModel Notifications { get; }

    public BindingList<TabViewModel> Tabs { get; }

    [ObservableProperty]
    public partial TabViewModel ActualTabItem { get; set; }

    public ObservableCollection<double> ZoomLevels { get; }

    [ObservableProperty]
    public partial double ActualZoomLevel { get; set; }

    partial void OnActualZoomLevelChanged(double value)
    {
        double fontSize = BaseFontSize * value;
        Application.Current.Resources["EditorFontSize"] = fontSize;
    }

    [RelayCommand(CanExecute = nameof(CanNewTab))]
    public void NewTab()
        => CreateStartPage();

    private bool CanNewTab()
        => Client.IsConnected;

    [RelayCommand]
    public void ExitApp()
        => Application.Current.Shutdown();

    void IRecipient<Messages.ReplaceTabContent>.Receive(Messages.ReplaceTabContent message)
    {
        int index = Tabs.IndexOf(ActualTabItem);
        Tabs[index].TabTitle = message.TabTitle;
        Tabs[index].Content = message.ViewModel;
        ActualTabItem = Tabs[index];
    }

    void IRecipient<Messages.CloseCurrentTab>.Receive(Messages.CloseCurrentTab message)
    {
        var tabToRemove = Tabs.FirstOrDefault(t => t.Content == message.ViewModel);
        if (tabToRemove != null)
        {
            int index = Tabs.IndexOf(tabToRemove);
            Tabs.Remove(tabToRemove);
            if (index >= 0 && index < Tabs.Count - 1)
            {
                ActualTabItem = Tabs[index];
            }
            else
            {
                CreateStartPage();
            }
        }
    }

    void IRecipient<Messages.IsConnectedChanged>.Receive(Messages.IsConnectedChanged message)
        => NewTabCommand.NotifyCanExecuteChanged();
}
