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
    IRecipient<Messages.ReplaceTabContentMessage>,
    IRecipient<Messages.CloseCurrentTabMessage>,
    IRecipient<Messages.IsConnectedChangedMessage>,
    IRecipient<RequestMessages.RequestClientMessage>
{
    private const int BaseFontSize = 16;

    public MainWindowViewModel(IDialogs dialogs)
    {
        Client = new ClientViewModel(dialogs);
        Tabs = new BindingList<TabViewModel>();
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        ActualZoomLevel = 1.0;
        CreateStartPage();
        WeakReferenceMessenger.Default.Register<Messages.ReplaceTabContentMessage>(this);
        WeakReferenceMessenger.Default.Register<Messages.CloseCurrentTabMessage>(this);
        WeakReferenceMessenger.Default.Register<Messages.IsConnectedChangedMessage>(this);
        WeakReferenceMessenger.Default.Register<RequestMessages.RequestClientMessage>(this);
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
        => CreateTab("Start page", new StartPageViewModel());

    public BindingList<TabViewModel> Tabs { get; }

    public TabViewModel ActualTabItem { get; set; }

    public ObservableCollection<double> ZoomLevels { get; }

    [ObservableProperty]
    public partial double ActualZoomLevel { get; set; }

    partial void OnActualZoomLevelChanged(double value)
    {
        double fontSize = BaseFontSize * value;
        Application.Current.Resources["EditorFontSize"] = fontSize;
    }

    public void Dispose()
    {
        Client.Dispose();
    }

    public ClientViewModel Client { get; }

    [RelayCommand(CanExecute = nameof(CanNewTab))]
    public void NewTab() 
        => CreateStartPage();

    private bool CanNewTab()
        => Client.IsConnected;

    [RelayCommand]
    public void ExitApp()
        => Application.Current.Shutdown();

    void IRecipient<Messages.ReplaceTabContentMessage>.Receive(Messages.ReplaceTabContentMessage message)
    {
        int index = Tabs.IndexOf(ActualTabItem);
        Tabs[index].TabTitle = message.tabTitle;
        Tabs[index].Content = message.ViewModel;
        ActualTabItem.TabTitle = message.tabTitle;
        ActualTabItem.Content = message.ViewModel;
    }

    void IRecipient<Messages.CloseCurrentTabMessage>.Receive(Messages.CloseCurrentTabMessage message)
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

    void IRecipient<Messages.IsConnectedChangedMessage>.Receive(Messages.IsConnectedChangedMessage message)
        => NewTabCommand.NotifyCanExecuteChanged();

    void IRecipient<RequestMessages.RequestClientMessage>.Receive(RequestMessages.RequestClientMessage message)
        => message.Reply(Client);
}
