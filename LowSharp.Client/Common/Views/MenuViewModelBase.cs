using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Client.Common.Views;

internal partial class MenuViewModel : ObservableObject
{
    [ObservableProperty]
    public required partial string Header { get; set; }

    public ObservableCollection<MenuViewModel> Children { get; set; }

    public MenuViewModel()
    {
        Children = new ObservableCollection<MenuViewModel>();
    }
}
