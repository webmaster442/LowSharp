using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace LowSharp.Client.Comon.Views;

internal abstract partial class ViewModelWithMenus : ObservableObject
{
    public ObservableCollection<MenuViewModel> Menus { get; }

    protected ViewModelWithMenus()
    {
        Menus = new ObservableCollection<MenuViewModel>();
    }

    /// <summary>
    /// Close current tab containing the view
    /// </summary>
    public void Close()
        => WeakReferenceMessenger.Default.Send(new Messages.CloseCurrentTabMessage(this));

    public void ReplaceContents(string title, ViewModelWithMenus viewModel)
        => WeakReferenceMessenger.Default.Send(new Messages.ReplaceTabContentMessage(title, viewModel));
}
