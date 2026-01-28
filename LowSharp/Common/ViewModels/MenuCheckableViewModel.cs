using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Common.ViewModels;

internal sealed partial class MenuCheckableViewModel : MenuViewModel
{
    [ObservableProperty] 
    public partial bool IsChecked { get; set; }
}
