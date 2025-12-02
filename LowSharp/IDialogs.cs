using LowSharp.Core;

namespace LowSharp;

internal interface IDialogs
{
    public bool TryOpenCode(out (string filename, InputLanguage language) result);
    public void Information(string title, params IEnumerable<string> lines);
}