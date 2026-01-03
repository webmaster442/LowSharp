using System.Windows;
using System.Windows.Controls;

namespace LowSharp.Client.Common.Views;

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
