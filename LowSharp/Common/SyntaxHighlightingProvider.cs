using System.Windows.Markup;

using ICSharpCode.AvalonEdit.Highlighting;

namespace LowSharp.Common;

internal sealed class SyntaxHighlightingProvider : MarkupExtension
{
    public string Language { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Language) 
            || !App.CustomHighlightings.TryGetValue(Language, out IHighlightingDefinition? value))
        {
            return null!;
        }

        return value;
    }
}
