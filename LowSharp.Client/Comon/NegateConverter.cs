using System.Globalization;

namespace LowSharp.Client.Comon;

internal sealed class NegateConverter : ConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool value, CultureInfo culture)
        => !value;

    protected override bool ConvertToBack(bool value, CultureInfo culture)
        => !value;
}
