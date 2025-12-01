using System.Runtime.InteropServices;
using System.Windows.Markup;

namespace LowSharp.Converters;

internal sealed class DotnetInfo : MarkupExtension
{
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return $"{RuntimeInformation.FrameworkDescription}";
    }
}
