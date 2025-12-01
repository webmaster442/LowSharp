using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using ICSharpCode.AvalonEdit.Highlighting;

namespace LowSharp.Converters;

public sealed class InputLanguageToIHighlightingDefinition : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 => HighlightingManager.Instance.GetDefinition("C#"),
                1 => HighlightingManager.Instance.GetDefinition("VB"),
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
