using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using LowSharp.Server.ApiV1;

namespace LowSharp.Converters;

public sealed class OutputTypeToText : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is OutputCodeType type)
        {
            return type switch
            {
                OutputCodeType.LoweredCsharp => "C#",
                OutputCodeType.Il => "Intermediate Language (IL)",
                OutputCodeType.JitAsm => "Jit Assembly",
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