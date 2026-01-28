using System.Diagnostics;

using LowSharp.Common;
using LowSharp.Common.Dialogs;

namespace LowSharp;

internal sealed class DialogsImplementation : IDialogs
{
    private readonly MainWindow _mainWindow;

    public DialogsImplementation(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void ClientError(Exception failure)
        => Error("Client Error", failure.Message);

    public bool Confirm(string title, string message)
        => MessageDialog.Confirm(_mainWindow, title, message);

    public void Error(string title, string message)
        => MessageDialog.Show(_mainWindow, $"Error: {title}", message);

    public void Info(string title, string message)
        => MessageDialog.Show(_mainWindow, title, message);

    public void OpenUrl(string url)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        });
    }
}
