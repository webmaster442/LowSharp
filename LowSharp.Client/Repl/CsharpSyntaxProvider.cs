using System.Windows.Markup;

using ICSharpCode.AvalonEdit.Highlighting;

namespace LowSharp.Client.Repl;

internal sealed class CsharpSyntaxProvider : MarkupExtension
{
    private static IReadOnlyDictionary<string, IHighlightingDefinition>? s_highlighters;

    public static void SetHighlighters(IReadOnlyDictionary<string, IHighlightingDefinition> highlighters)
    {
        s_highlighters = highlighters;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
        => s_highlighters!["CsharpNext"];
}
