using System.Windows.Documents;
using System.Windows.Media;

using LowSharp.ApiV1.Evaluate;

namespace LowSharp.Client.Repl;

internal static class FlowDocumentFormatter
{
    private static readonly Dictionary<ForegroundColor, SolidColorBrush> ColorMap = new()
    {
        { ForegroundColor.Black,        new SolidColorBrush(Color.FromRgb(0x21, 0x22, 0x2C)) },
        { ForegroundColor.Blue,         new SolidColorBrush(Color.FromRgb(0xBD, 0x93, 0xF9)) },
        { ForegroundColor.Cyan,         new SolidColorBrush(Color.FromRgb(0x8B, 0xE9, 0xFD)) },
        { ForegroundColor.Green,        new SolidColorBrush(Color.FromRgb(0x50, 0xFA, 0x7B)) },
        { ForegroundColor.Purple,       new SolidColorBrush(Color.FromRgb(0xFF, 0x79, 0xC6)) },
        { ForegroundColor.Red,          new SolidColorBrush(Color.FromRgb(0xFF, 0x55, 0x55)) },
        { ForegroundColor.White,        new SolidColorBrush(Color.FromRgb(0xF8, 0xF8, 0xF2)) },
        { ForegroundColor.Yellow,       new SolidColorBrush(Color.FromRgb(0xF1, 0xFA, 0x8C)) },
        { ForegroundColor.BrightBlack,  new SolidColorBrush(Color.FromRgb(0x62, 0x72, 0xA4)) },
        { ForegroundColor.BrightBlue,   new SolidColorBrush(Color.FromRgb(0xD6, 0xAC, 0xFF)) },
        { ForegroundColor.BrightCyan,   new SolidColorBrush(Color.FromRgb(0xA4, 0xFF, 0xFF)) },
        { ForegroundColor.BrightGreen,  new SolidColorBrush(Color.FromRgb(0x69, 0xFF, 0x94)) },
        { ForegroundColor.BrightPurple, new SolidColorBrush(Color.FromRgb(0xFF, 0x92, 0xDF)) },
        { ForegroundColor.BrightRed,    new SolidColorBrush(Color.FromRgb(0xFF, 0x6E, 0x6E)) },
        { ForegroundColor.BrightWhite,  new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xFF)) },
        { ForegroundColor.BrightYellow, new SolidColorBrush(Color.FromRgb(0xFF, 0xFF, 0xA5)) },
    };

    public static void AppendFormattedText(FlowDocument document, ApiV1.Evaluate.FormattedText output)
    {
        Paragraph paragraph;
        if (document.Blocks.LastBlock is Paragraph lastParagraph)
        {
            paragraph = lastParagraph;
        }
        else
        {
            paragraph = new Paragraph();
            document.Blocks.Add(paragraph);
        }

        var run = new Run(output.Text);

        if (output.Color != ForegroundColor.Default)
        {
            run.Foreground = ColorMap[output.Color];
        }

        Inline inline = run;

        // Apply Bold / Italic
        if (output.IsBold)
            inline = new Bold(inline);

        if (output.IsItalic)
            inline = new Italic(inline);

        paragraph.Inlines.Add(inline);
    }
}
