using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Client.Comon;

internal partial class MenuViewModel : ObservableObject
{
    [ObservableProperty]
    public required partial string Title { get; set; }

    public ObservableCollection<MenuViewModel> Children { get; set; }

    protected MenuViewModel()
    {
        Children = new ObservableCollection<MenuViewModel>();
    }
}
