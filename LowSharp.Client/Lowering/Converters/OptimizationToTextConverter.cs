using System.Globalization;

using LowSharp.Client.Common;
using LowSharp.ApiV1.Lowering;

namespace LowSharp.Client.Lowering.Converters;

internal sealed class OptimizationToTextConverter : OneWayConverterBase<Optimization, string>
{
    protected override string ConvertFrom(Optimization value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            Optimization.Debug => "Debug",
            Optimization.Release => "Release",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
