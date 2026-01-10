using LowSharp.Client.Common.Views;

namespace LowSharp.Client.RegexTesting;

internal sealed partial class RegexTestingViewModel : ViewModelWithMenus
{
    public RegexTestingViewModel()
    {
        Options = new RegexOptionViewModel();
    }

    public RegexOptionViewModel Options { get; }
}
