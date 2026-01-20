using System.Globalization;
using System.Windows;

using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common;

namespace LowSharp.Client.Lowering.Converters;

internal class PreviewButtonVisibilityConverter : OneWayConverterBase<int, Visibility>
{
    protected override Visibility ConvertFrom(int value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            4 => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }
}
