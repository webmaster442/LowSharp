using System.Globalization;
using System.Windows;

using LowSharp.ApiV1.Lowering;
using LowSharp.Common.Converters;

namespace LowSharp.Lowering;

internal sealed class PreviewButtonVisibilityConverter : OneWayConverterBase<OutputCodeType, Visibility>
{
    protected override Visibility ConvertFrom(OutputCodeType value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            OutputCodeType.Nonmoml => Visibility.Visible,
            OutputCodeType.Mermaid => Visibility.Visible,
            _ => Visibility.Collapsed,
        };
    }
}
