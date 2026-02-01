using System.ComponentModel;
using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Common;


namespace LowSharp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new DialogsImplementation(this));
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void TabCloseClick(object sender, RoutedEventArgs e)
    {
        var index = MainTabControl.SelectedIndex;
        WeakReferenceMessenger.Default.Send(new Messages.CloseTabAtIndex(index));
    }
}
