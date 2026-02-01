using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace LowSharp.Common.Converters;

internal abstract class MultiConverterBase<TFrom, TTo> : MarkupExtension, IMultiValueConverter
    where TFrom : notnull where TTo : notnull
{
    private static TFrom[] SafeCast(object[] values)
    {
        TFrom[] results = new TFrom[values.Length];
        for (int i=0; i<values.Length; i++)
        {
            if (values[i] is TFrom casted)
                results[i] = casted;
            else
                results[i] = default(TFrom)!;
        }
        return results;
    }

    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values != null && values.Length > 0)
        {
            var converted = SafeCast(values);
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
