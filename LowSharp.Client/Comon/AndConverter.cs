using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Client.Comon;

internal class AndConverter: MultiConverterBase<bool, bool>
{
    protected override bool ConvertFrom(bool[] converted, CultureInfo culture)
        => converted.All(v => v);
}
