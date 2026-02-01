using System.Globalization;

using ICSharpCode.AvalonEdit.Highlighting;

using LowSharp.ApiV1.Lowering;
using LowSharp.Common.Converters;

namespace LowSharp.Lowering;

internal sealed class SelectedInputLanguageToHighlightingConverter
    : OneWayConverterBase<InputLanguage, IHighlightingDefinition>
{
    protected override IHighlightingDefinition ConvertFrom(InputLanguage value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            InputLanguage.Csharp => App.CustomHighlightings["CsharpNext"],
            InputLanguage.Visualbasic => App.CustomHighlightings["VBNext"],
            InputLanguage.Fsharp => App.CustomHighlightings["F#"],
            _ => null!
        };
    }
}
