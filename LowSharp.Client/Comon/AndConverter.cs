using System.Globalization;

namespace LowSharp.Client.Comon;

internal class AndConverter: MultiConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool[] converted, CultureInfo culture)
        => converted.All(v => v);
}
