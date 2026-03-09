using System.Globalization;

using ICSharpCode.AvalonEdit.Highlighting;

using LowSharp.ApiV1.Lowering;
using LowSharp.Common.Converters;

namespace LowSharp.Lowering;

internal sealed class SelectedOutputTypeToHighlightingConverter
    : OneWayConverterBase<OutputCodeType, IHighlightingDefinition>
{
    protected override IHighlightingDefinition ConvertFrom(OutputCodeType value, object parameter, CultureInfo culture)
    {
        return value switch
        {
            OutputCodeType.Il => App.CustomHighlightings["ILAsm"],
            OutputCodeType.Loweredcsharp => App.CustomHighlightings["CsharpNext"],
            OutputCodeType.JitasmIntel => App.CustomHighlightings["x64"],
            OutputCodeType.JitasmAtnt => App.CustomHighlightings["x64"],
            OutputCodeType.Syntaxtreejson => App.CustomHighlightings["JsonNext"],
            OutputCodeType.Nonmoml => App.CustomHighlightings["nomnoml"],
            _ => null!,
        };
    }
}
