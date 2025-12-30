using System.Windows;

using CommunityToolkit.Mvvm.Messaging;

using Grpc.Net.Client;

namespace LowSharp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var channel = GrpcChannel.ForAddress("https://localhost:7042");
        var client = new Server.ApiV1.Lowerer.LowererClient(channel);

        DataContext = new MainWindowViewModel(new Dialogs(), client);
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