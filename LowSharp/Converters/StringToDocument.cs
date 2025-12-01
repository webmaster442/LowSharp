using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

using ICSharpCode.AvalonEdit.Document;

namespace LowSharp.Converters;

public sealed class StringToDocument : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is string str
            ? new TextDocument(str)
            : Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is IDocument document
            ? document.Text
            : Binding.DoNothing;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => this;
}