using System.Globalization;

using LowSharp.Client.Common;
using LowSharp.Lowering.ApiV1;

namespace LowSharp.Client.Lowering.Converters;

internal sealed class OutputCodeTypeToStringConverter : OneWayConverterBase<OutputCodeType, string>
{
    protected override string ConvertFrom(OutputCodeType value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            OutputCodeType.LoweredCsharp => "C# (Lowered)",
            OutputCodeType.Il => "IL",
            OutputCodeType.JitAsm => "JIT ASM",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
