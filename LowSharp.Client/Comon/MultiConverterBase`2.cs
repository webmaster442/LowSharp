using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Client.Comon;

internal abstract class MultiConverterBase<TFrom, TTo> : MarkupExtension, IMultiValueConverter
    where TFrom : notnull where TTo : notnull
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length > 0)
        {
            var converted = values.Cast<TFrom>().ToArray();
            return ConvertFrom(converted, culture);
        }
        return Binding.DoNothing;
    }

    protected abstract TTo ConvertFrom(TFrom[] converted, CultureInfo culture);

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => null!;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
