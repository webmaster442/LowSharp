using System.Globalization;

namespace LowSharp.Client.Comon;

internal sealed class NegateConverter : ConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool value, object parameter, CultureInfo culture)
        => !value;

    protected override bool ConvertToBack(bool value, object parameter, CultureInfo culture)
        => !value;
}
