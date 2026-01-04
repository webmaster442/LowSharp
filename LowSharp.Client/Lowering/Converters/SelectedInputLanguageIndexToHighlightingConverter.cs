using System.Globalization;

using ICSharpCode.AvalonEdit.Highlighting;

using LowSharp.Client.Common;

namespace LowSharp.Client.Lowering.Converters;

internal sealed class SelectedInputLanguageIndexToHighlightingConverter
    : OneWayConverterBase<int, IHighlightingDefinition>
{
    private static IReadOnlyDictionary<string, IHighlightingDefinition>? s_highlighters;

    public static void SetHighlighters(IReadOnlyDictionary<string, IHighlightingDefinition> highlighters)
    {
        s_highlighters = highlighters;
    }

    protected override IHighlightingDefinition ConvertFrom(int value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            0 => s_highlighters!["CsharpNext"],
            1 => s_highlighters!["VBNext"],
            2 => s_highlighters!["F#"],
            _ => null!
        };
    }
}
