using System.Globalization;

using LowSharp.Client.Common;
using LowSharp.Lowering.ApiV1;

namespace LowSharp.Client.Lowering.Converters;

internal sealed class InputLanguageToTextConverter : OneWayConverterBase<InputLanguage, string>
{
    protected override string ConvertFrom(InputLanguage value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            InputLanguage.Csharp => "C#",
            InputLanguage.VisualBasic => "VB.NET",
            InputLanguage.Fsharp => "F#",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }
}
