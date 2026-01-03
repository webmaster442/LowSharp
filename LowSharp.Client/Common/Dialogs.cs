using System;
using System.Collections.Generic;
using System.Text;

using LowSharp.Client.Common.Views;
using LowSharp.Lowering.ApiV1;

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

    public async Task Error(string title, string message)
    {
        await _mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, new MetroDialogSettings
        {
            DefaultButtonFocus = MessageDialogResult.Affirmative,
            ColorScheme = MetroDialogColorScheme.Accented,
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
                ? InputLanguage.VisualBasic
                : filename.EndsWith(".fs", StringComparison.OrdinalIgnoreCase)
                    ? InputLanguage.Fsharp
                    : InputLanguage.Csharp;
            result = (filename, language);
            return true;
        }
        result = default;
        return false;
    }
}
