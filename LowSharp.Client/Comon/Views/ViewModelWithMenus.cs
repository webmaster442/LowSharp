using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Client.Comon.Views;

internal abstract partial class ViewModelWithMenus : ObservableObject
{
    public ObservableCollection<MenuViewModel> Menus { get; }

    protected ViewModelWithMenus()
    {
        Menus = new ObservableCollection<MenuViewModel>();
    }
}
