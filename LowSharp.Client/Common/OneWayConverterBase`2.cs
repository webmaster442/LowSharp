using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Client.Common;

internal abstract class OneWayConverterBase<TFrom, TTo> : MarkupExtension, IValueConverter
    where TFrom : notnull where TTo : notnull
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TFrom fromValue
            ? ConvertFrom(fromValue, parameter, culture)
            : Binding.DoNothing;
    }

    protected abstract TTo ConvertFrom(TFrom value, object parameter, CultureInfo culture);

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
