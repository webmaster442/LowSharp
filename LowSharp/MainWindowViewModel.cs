using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Common;
using LowSharp.Common.ViewModels;
using LowSharp.Shell;

namespace LowSharp;

internal sealed partial class MainWindowViewModel :
    ObservableObject,
    IDisposable,
    IRecipient<Messages.ReplaceTabContent>,
    IRecipient<Messages.CloseCurrentTab>,
    IRecipient<Messages.IsConnectedChanged>,
    IRecipient<Messages.CloseTabAtIndex>
{
    private const int BaseFontSize = 16;
    private readonly IDialogs _dialogs;

    public MainWindowViewModel(IDialogs dialogs)
    {
        _dialogs = dialogs;
        Client = new ClientViewModel(dialogs);
        Tabs = new BindingList<TabViewModel>();
        ZoomLevels = new ObservableCollection<double>([0.2, 0.5, 0.7, 1.0, 1.2, 1.5, 2.0, 4.0]);
        ActualZoomLevel = 1.0;
        CreateStartPage();
        WeakReferenceMessenger.Default.Register<Messages.ReplaceTabContent>(this);
        WeakReferenceMessenger.Default.Register<Messages.CloseCurrentTab>(this);
        WeakReferenceMessenger.Default.Register<Messages.IsConnectedChanged>(this);
        WeakReferenceMessenger.Default.Register<Messages.CloseTabAtIndex>(this);
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
        => CreateTab("Start page", new StartPageViewModel(Client.Client, _dialogs));

    public void Dispose()
    {
        Client.Dispose();
    }

    public ClientViewModel Client { get; }

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

    [RelayCommand]
    public void VisitWebsite()
        => _dialogs.OpenUrl("https://github.com/webmaster442/LowSharp");

    [RelayCommand]
    public async Task ViewChanges()
    {
        using var stream = typeof(MainWindowViewModel).Assembly.GetManifestResourceStream("LowSharp.changelog.md");
        var content = new StreamReader(stream!).ReadToEnd();
        _dialogs.Info("Change log", content);
    }

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

    void IRecipient<Messages.CloseTabAtIndex>.Receive(Messages.CloseTabAtIndex message)
    {
        if (message.Index >= 0 && message.Index < Tabs.Count)
            Tabs.RemoveAt(message.Index);
    }
}
