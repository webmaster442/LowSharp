using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Converters;

public sealed class NegateBooleanConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b
            ? !b 
            : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool b
            ? !b 
            : Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
