using LowSharp.Core;

namespace LowSharp;

internal interface IDialogs
{
    public bool TryOpenCode(out (string filename, InputLanguage language) result);
}