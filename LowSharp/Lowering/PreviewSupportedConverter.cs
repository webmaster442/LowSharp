using System.Globalization;

using LowSharp.ApiV1.Lowering;
using LowSharp.Common.Converters;

namespace LowSharp.Lowering;

internal sealed class PreviewSupportedConverter
    : OneWayConverterBase<OutputCodeType, bool>
{
    protected override bool ConvertFrom(OutputCodeType value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            OutputCodeType.Mermaid => true,
            OutputCodeType.Nonmoml => true,
            _ => false,
        };
    }
}
