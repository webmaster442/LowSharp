using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using LowSharp.Client.Common.Views;

namespace LowSharp.Client.RegexTesting;

internal sealed partial class RegexTestingViewModel : ViewModelWithMenus
{
    public RegexTestingViewModel(IClient client)
    {
        Options = new RegexOptionViewModel();
        IsMatchMode = true;
        Input = string.Empty;
        Pattern = string.Empty;
        Replacement = string.Empty;
        Result = string.Empty;
    }

    [ObservableProperty]
    public partial bool IsMatchMode { get; set; }

    [ObservableProperty]
    public partial bool IsReplaceMode { get; set; }

    [ObservableProperty]
    public partial bool IsSplitMode { get; set; }

    public RegexOptionViewModel Options { get; }

    [ObservableProperty]
    public partial string Pattern { get; set; }

    [ObservableProperty]
    public partial string Input { get; set; }

    [ObservableProperty]
    public partial string Replacement { get; set; }

    [ObservableProperty]
    public partial string Result { get; set; }

    [RelayCommand]
    public async Task Execute()
    {
        
    }
}
