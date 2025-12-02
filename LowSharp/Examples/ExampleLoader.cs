using System.Collections.ObjectModel;
using System.Xml.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace LowSharp.Examples;

internal sealed class ExamplesViewModel : ObservableObject
{
    private readonly ExamplesRoot _root;

    public ObservableCollection<Example> Csharp { get; }

    public ExamplesViewModel()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ExamplesRoot), new XmlRootAttribute("examples"));
        using (var stream = typeof(ExamplesViewModel).Assembly.GetManifestResourceStream("LowSharp.Examples.Examples.xml")!)
        {
            _root = (ExamplesRoot)serializer.Deserialize(stream)!;
        }
        Csharp = new ObservableCollection<Example>(_root.Examples.Where(e => e.Type == "csharp"));
    }
}
