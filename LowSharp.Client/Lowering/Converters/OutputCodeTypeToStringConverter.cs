using System.Globalization;

using LowSharp.ApiV1.Lowering;
using LowSharp.Client.Common;

namespace LowSharp.Client.Lowering.Converters;

internal sealed class OutputCodeTypeToStringConverter : OneWayConverterBase<OutputCodeType, string>
{
    protected override string ConvertFrom(OutputCodeType value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            OutputCodeType.Loweredcsharp => "C# (Lowered)",
            OutputCodeType.Il => "IL",
            OutputCodeType.Jitasm => "JIT ASM",
            OutputCodeType.Syntaxtreejson => "Syntax Tree (JSON)",
            OutputCodeType.Nonmoml => "Nomnoml",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
