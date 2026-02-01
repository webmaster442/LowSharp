using System.Globalization;

namespace LowSharp.Common.Converters;

internal class AndConverter : MultiConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool[] converted, CultureInfo culture)
        => converted.All(v => v);
}
