using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;

namespace Lowsharp.Server.Interactive;

internal static class CsharpFormatter
{
    public static async IAsyncEnumerable<TextWithFormat> Format(Document document)
    {
        var text = (await document.GetTextAsync()).ToString();

        var classified = await Classifier.GetClassifiedSpansAsync(document, TextSpan.FromBounds(0, text.Length));

        var classifications = classified.GroupBy(c => c.TextSpan);

        foreach (var classification in classifications)
        {
            yield return Format(text, classification);
            yield return " ";
        }
    }

    private static TextWithFormat Format(string text, IGrouping<TextSpan, ClassifiedSpan> classification)
    {
        var type = classification.Select(c => c.ClassificationType).FirstOrDefault();
        return new TextWithFormat
        {
            Text = text.Substring(classification.Key.Start, classification.Key.End - classification.Key.Start),
            Color = GetColor(type),
            Bold = IsBold(type),
        };
    }

    private static bool IsBold(string? type)
        => type == ClassificationTypeNames.Keyword;

    private static ForegroundColor GetColor(string? type)
    {
        if (string.IsNullOrEmpty(type))
            return ForegroundColor.Default;

        return type switch
        {
            ClassificationTypeNames.Comment => ForegroundColor.Green,
            ClassificationTypeNames.Keyword => ForegroundColor.Blue,
            _ => ForegroundColor.Default,
        };
    }
}
