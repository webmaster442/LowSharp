using System.Text;

using ColorCode;
using ColorCode.Common;
using ColorCode.Parsing;
using ColorCode.Styling;

using Spectre.Console;

namespace LowSharp.Cli;

internal sealed class ConsoleFormatter : CodeColorizerBase
{
    private TextWriter? _writer;

    public ConsoleFormatter(StyleDictionary? Styles = null, ILanguageParser? languageParser = null) : base(Styles, languageParser)
    {
    }

    public string GetHtmlString(string sourceCode, ILanguage language)
    {
        var buffer = new StringBuilder(sourceCode.Length * 2);

        using (TextWriter writer = new StringWriter(buffer))
        {
            _writer = writer;
            languageParser.Parse(sourceCode, language, (parsedSourceCode, captures) => Write(parsedSourceCode, captures));
            writer.Flush();
        }

        return buffer.ToString();
    }

    private static void GetStyleInsertionsForCapturedStyle(Scope scope,
                                                           ICollection<TextInsertion> styleInsertions)
    {
        styleInsertions.Add(new TextInsertion
        {
            Index = scope.Index,
            Scope = scope
        });

        foreach (Scope childScope in scope.Children)
        {
            GetStyleInsertionsForCapturedStyle(childScope, styleInsertions);
        }

        styleInsertions.Add(new TextInsertion
        {
            Index = scope.Index + scope.Length,
            Text = "[/]"
        });
    }

    private void BuildSpanForCapturedStyle(Scope scope)
    {
        string foreground = string.Empty;
        string background = string.Empty;
        bool italic = false;
        bool bold = false;

        if (Styles.Contains(scope.Name))
        {
            ColorCode.Styling.Style style = Styles[scope.Name];

            foreground = style.Foreground;
            background = style.Background;
            italic = style.Italic;
            bold = style.Bold;
        }

        WriteElementStart(foreground, background, italic, bold);
    }

    private string? ToColor(string color)
    {
        if (color == null) return null;

        const int length = 6;
        int start = color.Length - length;
        string hex = color.Substring(start, length);
        return color.Substring(start, length) switch
        {
            "000000" => "black",
            "FFFFFF" => "white",
            "FF0000" => "red",
            "00FF00" => "green",
            "0000FF" => "blue",
            "FFFF00" => "yellow",
            "00FFFF" => "cyan",
            "FF00FF" => "magenta",
            _ => $"#{hex}"
        };
    }


    private void WriteElementStart(string? foreground = null,
                                   string? background = null,
                                   bool italic = false,
                                   bool bold = false)
    {
        if (!string.IsNullOrWhiteSpace(foreground)
            || !string.IsNullOrWhiteSpace(background)
            || italic
            || bold)
        {
            _writer?.Write("[");

            if (!string.IsNullOrWhiteSpace(foreground))
                _writer?.Write("{0}", ToColor(foreground));

            if (!string.IsNullOrWhiteSpace(background))
                _writer?.Write(" on {0}", ToColor(background));

            if (italic)
                _writer?.Write(" italic");

            if (bold)
                _writer?.Write(" bold");

            _writer?.Write("]");
        }

    }

    protected override void Write(string parsedSourceCode, IList<Scope> scopes)
    {
        var styleInsertions = new List<TextInsertion>();

        foreach (Scope scope in scopes)
            GetStyleInsertionsForCapturedStyle(scope, styleInsertions);

        styleInsertions.SortStable((x, y) => x.Index.CompareTo(y.Index));

        int offset = 0;

        foreach (TextInsertion styleInsertion in styleInsertions)
        {
            var text = parsedSourceCode[offset..styleInsertion.Index];
            _writer?.Write(text.EscapeMarkup());

            if (string.IsNullOrEmpty(styleInsertion.Text))
            {
                BuildSpanForCapturedStyle(styleInsertion.Scope);
            }
            else
            {
                _writer?.Write(styleInsertion.Text);
            }

            offset = styleInsertion.Index;
        }

        _writer?.Write(parsedSourceCode.Substring(offset).EscapeMarkup());
    }
}