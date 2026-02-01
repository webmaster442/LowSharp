using System.IO;
using System.Windows;
using System.Xml;

using ICSharpCode.AvalonEdit.Highlighting;

using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace LowSharp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static readonly Dictionary<string, IHighlightingDefinition> CustomHighlightings
        = new Dictionary<string, IHighlightingDefinition>();

    protected override void OnStartup(StartupEventArgs e)
    {
        var customSyntaxes = typeof(App).Assembly
            .GetManifestResourceNames()
            .Where(x => Path.GetExtension(x) == ".xshd");

        foreach (var syntax in customSyntaxes)
        {
            using var stream = typeof(App).Assembly.GetManifestResourceStream(syntax)!;
            using var xmlReader = XmlReader.Create(stream);
            var xshd = HighlightingLoader.Load(xmlReader, HighlightingManager.Instance);

            var name = xshd.Name;
            CustomHighlightings.Add(name, xshd);
        }
    }
}

