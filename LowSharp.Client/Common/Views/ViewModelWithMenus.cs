using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace LowSharp.Client.Common.Views;

internal abstract class ViewModelWithMenus : ObservableObject
{
    public ObservableCollection<MenuViewModel> Menus { get; }

    public virtual Task InitializeAsync()
        => Task.CompletedTask;

    protected ViewModelWithMenus()
    {
        Menus = new ObservableCollection<MenuViewModel>();
    }

    /// <summary>
    /// Close current tab containing the view
    /// </summary>
    public void Close()
        => WeakReferenceMessenger.Default.Send(new Messages.CloseCurrentTab(this));

    public void ReplaceContents(string title, ViewModelWithMenus viewModel)
        => WeakReferenceMessenger.Default.Send(new Messages.ReplaceTabContent(title, viewModel));
}
