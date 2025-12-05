using System.Text;

using ColorCode;

namespace LowSharp.Core.Internals;

internal static class HtmlExporter
{
    internal static string Create(string code, InputLanguage inputLanguage, string csharpCode, string ilCode)
    {
        var formatter = new HtmlClassFormatter();

        StringBuilder sb = new((code.Length + csharpCode.Length + ilCode.Length) * 2);

        var inHtml = formatter.GetHtmlString(code, inputLanguage.ToColorCodeLanguage());
        var outCsHtml = formatter.GetHtmlString(csharpCode, Languages.CSharp);

        sb.AppendLine(
            """
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>LowSharp Export</title>
              <style>
              body { background-color: #f0f0f0; }
              summary { font-size: 2em; }
              #container { max-width: 1100px; margin: auto; }
              </style>
            """);

        sb.AppendLine($"""
            <style>{formatter.GetCSSString()}</style>
            </head>
            <body>
            <div id="container">
            """);

        sb.AppendLine($"""
            <details>
                <summary>Input code</summary>
                {inHtml}
            </details>
            """);

        sb.AppendLine($"""
            <details>
                <summary>Lowered C#</summary>
                {outCsHtml}
            </details>
            """);


        sb.AppendLine($"""
            <details>
                <summary>Lowered IL Code</summary>
                <pre><code>{ilCode}</code></pre>
            </details>
            """);

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }
}
