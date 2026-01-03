using System.Globalization;

namespace LowSharp.Client.Common;

internal sealed class ZoomLevelToTextConverter: OneWayConverterBase<double, string>
{
    protected override string ConvertFrom(double value, object parameter, CultureInfo culture)
    {
        return $"{value:P0}";
    }
}
