using System;
using System.Collections.Generic;
using System.Text;

using LowSharp.Client.Comon.Views;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace LowSharp.Client.Comon;

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
}
