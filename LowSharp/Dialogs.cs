
using System.Windows;

using LowSharp.Core;

using Microsoft.Win32;

namespace LowSharp;

internal sealed class Dialogs : IDialogs
{
    public void Error(string title, params IEnumerable<string> lines)
    {
        var content = string.Join(Environment.NewLine, lines);
        MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public string? ExportDialog()
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "HTML Files (*.html)|*.html",
            Title = "Export results",
        };

        return saveFileDialog.ShowDialog() == true
            ? saveFileDialog.FileName
            : null;
    }

    public void Information(string title, params IEnumerable<string> lines)
    {
        var content = string.Join(Environment.NewLine, lines);
        MessageBox.Show(content, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool TryOpenCode(out (string filename, InputLanguage language) result)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "C# Files (*.cs)|*.cs|VB Files (*.vb)|*.vb|All Files (*.*)|*.*",
            Title = "Open Code File",
        };
        if (openFileDialog.ShowDialog() == true)
        {
            string filename = openFileDialog.FileName;
            InputLanguage language = filename.EndsWith(".vb", StringComparison.OrdinalIgnoreCase)
                ? InputLanguage.VisualBasic
                : InputLanguage.Csharp;
            result = (filename, language);
            return true;
        }
        result = default;
        return false;
    }
}