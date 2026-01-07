using System.Data;
using System.IO;
using System.Windows;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

using LowSharp.Client.Lowering.Converters;
using LowSharp.Client.Repl;

namespace LowSharp.Client;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        var customSyntaxes = typeof(App).Assembly
            .GetManifestResourceNames()
            .Where(x => Path.GetExtension(x) == ".xshd");

        Dictionary<string, IHighlightingDefinition> definitions = new();

        foreach (var syntax in customSyntaxes)
        {
            using var stream = typeof(App).Assembly.GetManifestResourceStream(syntax)!;
            using var xmlReader = XmlReader.Create(stream);
            var xshd = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);

            var name = xshd.Name;
            definitions.Add(name, xshd);
        }

        CsharpSyntaxProvider.SetHighlighters(definitions);
        SelectedInputLanguageIndexToHighlightingConverter.SetHighlighters(definitions);
        SelectedOutputTypeIndexToHighlightingConverter.SetHighlighters(definitions);
    }
}

