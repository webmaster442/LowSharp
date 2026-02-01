using System.Globalization;
using System.Windows;

namespace LowSharp.Common.Converters;

internal class WindowStateButtonContentConverter : OneWayConverterBase<WindowState, string>
{
    protected override string ConvertFrom(WindowState value, object parameter, CultureInfo culture)
    {
        return value == WindowState.Maximized 
            ? "\xE923" 
            : "\xE922";
    }
}
