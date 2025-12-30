using LowSharp.Server.ApiV1;

namespace LowSharp;

internal interface IDialogs
{
    public bool TryOpenCode(out (string filename, InputLanguage language) result);
    public void Information(string title, params IEnumerable<string> lines);
    public void Error(string title, params IEnumerable<string> lines);
    public string? ExportDialog();
}