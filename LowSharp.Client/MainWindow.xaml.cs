using System.ComponentModel;
using System.Windows.Controls;

using CommunityToolkit.Mvvm.Messaging;

using LowSharp.Client.Common;

using MahApps.Metro.Controls;

namespace LowSharp.Client;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public sealed partial class MainWindow : MetroWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new Dialogs(this));
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);
        if (DataContext is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is TabControl tabControl)
        {
            WeakReferenceMessenger.Default.Send(new Messages.TabIndexChanged(tabControl.SelectedIndex));
        }
    }
}
