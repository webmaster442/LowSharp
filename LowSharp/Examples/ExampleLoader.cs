using System.Xml.Serialization;

namespace LowSharp.Examples;

internal sealed class ExampleLoader
{
    private readonly ExamplesRoot _root;

    public ExampleLoader()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ExamplesRoot));
        using (var stream = typeof(ExampleLoader).Assembly.GetManifestResourceStream("LowSharp.Examples.Examples.xml")!)
        {
            _root = (ExamplesRoot)serializer.Deserialize(stream)!;
        }
    }

    public IEnumerable<Example> GetCsharpExamples()
    {
        return _root.Examples.Where(e => e.Type == "Csharp");
    }

}
