using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using ICSharpCode.AvalonEdit.Highlighting;

namespace LowSharp.Converters;

public sealed class OutputLanguageToIHighlightingDefinition : MarkupExtension, IValueConverter
{
    private static IReadOnlyDictionary<string, IHighlightingDefinition>? s_highlighters;

    public static void SetHighlighters(IReadOnlyDictionary<string, IHighlightingDefinition> highlighters)
    {
        s_highlighters = highlighters;
    }


    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 => s_highlighters!["CsharpNext"],
                1 => s_highlighters!["ILAsm"],
                _ => Binding.DoNothing,
            };
        }
        return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}