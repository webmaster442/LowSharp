using System.ComponentModel;

using LowSharp.Client.Comon;

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
}