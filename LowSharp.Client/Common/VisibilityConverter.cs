using System.Globalization;
using System.Windows;

namespace LowSharp.Client.Common;

internal sealed class VisibilityConverter : ConverterBase<bool, Visibility>
{
    protected override Visibility ConvertFrom(bool value, object parameter, CultureInfo culture)
    {
        if (parameter is not null)
            return value ? Visibility.Collapsed : Visibility.Visible;
        else
            return value ? Visibility.Visible : Visibility.Collapsed;
    }

    protected override bool ConvertToBack(Visibility value, object parameter, CultureInfo culture)
    {
        if (parameter is not null)
            return value != Visibility.Visible;
        else
            return value == Visibility.Visible;
    }
}
