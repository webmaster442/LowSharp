using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Converters;

public sealed class InputLanguageToText : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Core.InputLanguage lang)
        {
            return lang switch
            {
                Core.InputLanguage.Csharp => "C#",
                Core.InputLanguage.VisualBasic => "Visual Basic.NET",
                Core.InputLanguage.FSharp => "F#",
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