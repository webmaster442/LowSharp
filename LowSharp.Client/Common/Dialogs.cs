using System.Diagnostics;
using System.IO;
using System.Windows.Media;

using CommunityToolkit.Mvvm.Messaging;

using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common.Views;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

using Microsoft.Win32;

namespace LowSharp.Client.Common;

internal sealed class Dialogs : IDialogs
{
    private readonly MetroWindow _mainWindow;

    public Dialogs(MetroWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public Task ClientError(Exception failure)
        => Error("Client Error", failure.Message);

    public async Task Error(string title, string message)
    {
        await _mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings
        {
            DefaultButtonFocus = MessageDialogResult.Affirmative,
            ColorScheme = MetroDialogColorScheme.Accented,
        });
    }

    public async Task<bool> Confirm(string title, string message)
    {
        var result = await _mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings
        {
            DefaultButtonFocus = MessageDialogResult.Negative,
            ColorScheme = MetroDialogColorScheme.Theme,
        });
        return result == MessageDialogResult.Affirmative;
    }

    public async Task Info(string title, string message)
    {
        await _mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings
        {
            DefaultButtonFocus = MessageDialogResult.Affirmative,
            ColorScheme = MetroDialogColorScheme.Theme,
        });
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

    public void Notify(string message, int validityInSeconds)
        => WeakReferenceMessenger.Default.Send(new Messages.Notification(message, TimeSpan.FromSeconds(validityInSeconds)));

    public void OpenUrl(string url)
    {
        using var process = new Process();
        process.StartInfo.FileName = url;
        process.StartInfo.UseShellExecute = true;
        process.Start();
    }

    public async Task ChangeLog()
    {
        using var stream = typeof(Dialogs).Assembly.GetManifestResourceStream("LowSharp.Client.changelog.md");
        var content = new StreamReader(stream!).ReadToEnd();
        await Info("Change Log", content);
    }
}
