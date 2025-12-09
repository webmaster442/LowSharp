using System.Text;

using LowSharp.Core;

using Spectre.Console;

namespace LowSharp.Cli.Highlighter;

internal abstract class KeywordHighlighter
{
    public static KeywordHighlighter Create(OutputLanguage language)
    {
        return language switch
        {
            OutputLanguage.Csharp => new CSharpKeywordHighlighter(),
            OutputLanguage.IL => new VisualBasicKeywordHighlighter(),
            _ => throw new NotSupportedException($"Output language '{language}' is not supported for keyword highlighting."),
        };
    }

    private readonly HashSet<string> _keywords;

    protected KeywordHighlighter()
    {
        _keywords = new HashSet<string>(GetKeywords(), Comparision);
    }

    protected abstract IEqualityComparer<string> Comparision { get; }

    protected abstract IEnumerable<string> GetKeywords();

    public string HighlightKeyWords(string input, Color keywordColor)
    {
        StringBuilder builder = new StringBuilder(input.Length);
        using var reader = new StringReader(input);
        string? line;

        while ((line = reader.ReadLine()) != null)
        {
            var words = line.Split();
            foreach (var word in words)
            {
                if (_keywords.Contains(word))
                {
                    builder.Append($"[bold {keywordColor}]{word.EscapeMarkup()}[/]");
                }
                else
                {
                    builder.Append(word.EscapeMarkup());
                }
                builder.Append(' ');
            }
            builder.AppendLine();
        }

        return builder.ToString();
    }
}