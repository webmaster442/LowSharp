using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Client.Comon;

internal abstract class ConverterBase<TFrom, TTo> : MarkupExtension, IValueConverter
    where TFrom : notnull where TTo: notnull
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TFrom fromValue
            ? ConvertFrom(fromValue, culture)
            : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TTo toValue
            ? ConvertToBack(toValue, culture)
            : Binding.DoNothing;
    }

    protected abstract TTo ConvertFrom(TFrom value, CultureInfo culture);

    protected abstract TFrom ConvertToBack(TTo value, CultureInfo culture);

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
