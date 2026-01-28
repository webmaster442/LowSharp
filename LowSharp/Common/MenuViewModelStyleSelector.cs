using System.Windows;
using System.Windows.Controls;

using LowSharp.Common.ViewModels;

namespace LowSharp.Common;

internal sealed class MenuViewModelStyleSelector : StyleSelector
{
    public override Style SelectStyle(object item, DependencyObject container)
    {
        return item switch
        {
            MenuCheckableViewModel => (Application.Current.FindResource("MenuCheckableViewModelStyle") as Style)!,
            MenuCommandViewModel => (Application.Current.FindResource("MenuCommandViewModelStyle") as Style)!,
            _ => base.SelectStyle(item, container)
        };
    }
}
