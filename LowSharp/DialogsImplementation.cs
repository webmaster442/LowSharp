using System.Diagnostics;

using LowSharp.ApiV1.Lowering;
using LowSharp.Common;
using LowSharp.Common.Dialogs;

using Microsoft.Win32;

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

    public void OpenWebView(string title, Uri url)
    {
        var webWindow = new WebViewWindow();
        webWindow.Title = title;
        webWindow.ShowAndNavigateTo(_mainWindow, url);
    }


    public bool TryOpenCode(out (string filename, InputLanguage language) result)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "All supported (*cs;*.vb;*.fs)|*.cs;*.vb;*.fs|C# Files (*.cs)|*.cs|VB Files (*.vb)|*.vb|F# files (*.fs)|*.fs",
            Title = "Open Code File",
        };
        if (openFileDialog.ShowDialog() == true)
        {
            string filename = openFileDialog.FileName;
            InputLanguage language = filename.EndsWith(".vb", StringComparison.OrdinalIgnoreCase)
                ? InputLanguage.Visualbasic
                : filename.EndsWith(".fs", StringComparison.OrdinalIgnoreCase)
                    ? InputLanguage.Fsharp
                    : InputLanguage.Csharp;
            result = (filename, language);
            return true;
        }
        result = default;
        return false;
    }

    public bool TryOpen(string title, string filter, out string filename)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = filter,
            Title = title,
        };

        if (openFileDialog.ShowDialog() == true)
        {
            filename = openFileDialog.FileName;
            return true;
        }

        filename = string.Empty;
        return false;
    }

    public bool TrySave(string title, string filter, out string filename)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = filter,
            Title = title,
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            filename = saveFileDialog.FileName;
            return true;
        }

        filename = string.Empty;
        return false;
    }
}
