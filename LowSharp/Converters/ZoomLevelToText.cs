using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Converters;

public sealed class ZoomLevelToText : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is double d
            ? $"{(d * 100.0)} %"
            : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
