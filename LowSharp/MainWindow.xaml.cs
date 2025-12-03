using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

namespace LowSharp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainWindowViewModel(new Dialogs());
#pragma warning disable WPF0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        Application.Current.ThemeMode = ThemeMode.Light;
#pragma warning restore WPF0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

        WeakReferenceMessenger.Default.Register<Messages.GetInputCodeRequest>(this, (recipient, message) => message.Reply(Input.Document.Text));
        WeakReferenceMessenger.Default.Register<Messages.SetInputCodeRequest>(this, (recipient, message) => Input.Document.Text = message.Code);
    }

    public override void EndInit()
    {
        base.EndInit();
        ResizeMainWindowBasedOnResolution();
    }

    private void ResizeMainWindowBasedOnResolution()
    {
        if (SystemParameters.WorkArea.Width > 1920
            || SystemParameters.WorkArea.Height > 1080)
        {
            Width = Math.Min(Width * 1.3, SystemParameters.WorkArea.Width);
            Height = Math.Min(Height * 1.3, SystemParameters.WorkArea.Height);
        }
    }
}
