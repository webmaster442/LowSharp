using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using LowSharp.Server.ApiV1;

namespace LowSharp.Converters;

public sealed class InputLanguageToText : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is InputLanguage lang)
        {
            return lang switch
            {
                InputLanguage.Csharp => "C#",
                InputLanguage.VisualBasic => "Visual Basic.NET",
                InputLanguage.Fsharp => "F#",
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