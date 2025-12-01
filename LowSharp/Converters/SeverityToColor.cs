using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using LowSharp.Core;

namespace LowSharp.Converters;

public sealed class MessageSeverityToColor : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not MessageSeverity severity)
            return Binding.DoNothing;

        return severity switch
        {
            MessageSeverity.Info => new SolidColorBrush(Colors.Blue),
            MessageSeverity.Warning => new SolidColorBrush(Colors.Yellow),
            MessageSeverity.Error => new SolidColorBrush(Colors.Red),
            _ => Binding.DoNothing,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}
