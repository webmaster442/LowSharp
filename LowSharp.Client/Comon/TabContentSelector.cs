using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using LowSharp.Client.Comon.Views;

using Windows.Web.Syndication;

namespace LowSharp.Client.Comon;

internal class TabContentSelector : StyleSelector
{
    public override Style SelectStyle(object item, DependencyObject container)
    {
        if (item is TabViewModel tabVm)
        {
            if (tabVm.Content is StartPageViewModel)
            {
                return Application.Current.FindResource("TabStartPage") as Style;
            }
        }

        return base.SelectStyle(item, container);
    }
}
