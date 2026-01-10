using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LowSharp.Client.Common;

internal sealed class BooleanToGridLengthConverter : OneWayConverterBase<bool, GridLength>, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length ==  2
            && values[0] is bool isExpanded
            && values[1] is Visibility visibility)
        {
            if (visibility != Visibility.Visible)
                return GridLength.Auto;

            return isExpanded ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
        }

        return Binding.DoNothing;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        return null!;
    }

    protected override GridLength ConvertFrom(bool value, object parameter, CultureInfo culture)
    {
        return value ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
    }
}
