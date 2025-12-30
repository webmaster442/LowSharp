using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

using LowSharp.Server.ApiV1;

namespace LowSharp.Converters;

public sealed class DiagnosticSeveritySeverityToColor : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not DiagnosticSeverity severity)
            return Binding.DoNothing;

        return severity switch
        {
            DiagnosticSeverity.Info => new SolidColorBrush(Colors.Blue),
            DiagnosticSeverity.Warning => new SolidColorBrush(Colors.Yellow),
            DiagnosticSeverity.Error => new SolidColorBrush(Colors.Red),
            _ => Binding.DoNothing,
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => Binding.DoNothing;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}