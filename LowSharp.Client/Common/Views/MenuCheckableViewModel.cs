using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Client.Common.Views;

internal sealed partial class MenuCheckableViewModel : MenuViewModel
{
    [ObservableProperty]
    public partial bool IsChecked { get; set; }
}