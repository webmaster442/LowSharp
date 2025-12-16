using System.Net;
using System.Text;

namespace LowSharp.Core.Internals;

internal static class HtmlExporter
{
    internal static string Create(string code, InputLanguage inputLanguage, string csharpCode, string ilCode)
    {
        StringBuilder sb = new((code.Length + csharpCode.Length + ilCode.Length) * 2);

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
            <style>{GetContents("prism.css")}</style>
            <script>{GetContents("prism.js")}</script>
            </head>
            <body>
            <div id="container">
            """);

        sb.AppendLine($"""
            <details>
                <summary>Input code</summary>
                <pre><code class="{GetClassName(inputLanguage)}">{WebUtility.HtmlEncode(code)}</code></pre>
            </details>
            """);

        sb.AppendLine($"""
            <details>
                <summary>Lowered C#</summary>
                <pre><code class="language-csharp">{WebUtility.HtmlEncode(csharpCode)}</code></pre>
            </details>
            """);


        sb.AppendLine($"""
            <details>
                <summary>Lowered IL Code</summary>
                 <pre><code class="language-cil">{WebUtility.HtmlEncode(ilCode)}</code></pre>
            </details>
            """);

        sb.AppendLine("</div></body></html>");

        return sb.ToString();
    }

    private static string GetClassName(InputLanguage inputLanguage)
    {
        return inputLanguage switch
        {
            InputLanguage.Csharp => "language-csharp",
            InputLanguage.VisualBasic => "language-vbnet",
            InputLanguage.FSharp => "language-fsharp",
            _ => "language-plaintext",
        };
    }

    private static string GetContents(string resourceName)
    {
        using Stream? data = typeof(HtmlExporter).Assembly.GetManifestResourceStream($"LowSharp.Core.Resources.{resourceName}")
            ?? throw new InvalidOperationException($"Resource '{resourceName}' not found.");
        using var reader = new StreamReader(data);
        return reader.ReadToEnd();
    }
}