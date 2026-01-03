using System.Windows;
using System.Windows.Markup;

namespace LowSharp.Client.Common;

internal sealed class WindowSizeProvider : MarkupExtension
{
    public enum SizeType
    {
        Width,
        Height
    }

    public SizeType Type { get; set; }

    private const double HdWidth = 1280;
    private const double HdHeight = 720;
    private const double FullHdWidth = 1920;
    private const double FullHdHeight = 1080;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Type switch
        {
            SizeType.Width => GetWidth(),
            SizeType.Height => GetHeight(),
            _ => throw new NotSupportedException($"Unsupported size type: {Type}"),
        };
    }

    private static double GetHeight()
    {
        return SystemParameters.PrimaryScreenHeight > FullHdHeight
            ? FullHdHeight
            : HdHeight;
    }

    private static double GetWidth()
    {
        return SystemParameters.PrimaryScreenWidth > FullHdWidth
            ? FullHdWidth 
            : HdWidth;
    }
}
