using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Converters;

public sealed class OutputTypeToText : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Core.OutputLanguage type)
        {
            return type switch
            {
                Core.OutputLanguage.Csharp => "C#",
                Core.OutputLanguage.IL => "Intermediate Language (IL)",
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