using LowSharp.Core;

using Microsoft.Win32;

namespace LowSharp;

public class Dialogs : IDialogs
{
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