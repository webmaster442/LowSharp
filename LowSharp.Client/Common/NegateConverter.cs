using System.Globalization;

namespace LowSharp.Client.Common;

internal sealed class NegateConverter : TwoWayConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool value, object parameter, CultureInfo culture)
        => !value;

    protected override bool ConvertToBack(bool value, object parameter, CultureInfo culture)
        => !value;
}
