using System.Collections;
using System.Xml.Serialization;

namespace Lowsharp.Server.CodeExamples;

internal sealed class ExampleProvider : IEnumerable<Example>
{
    private readonly Example[] _examples;

    public ExampleProvider()
    {
        using var stream = typeof(ExampleProvider).Assembly.GetManifestResourceStream("Lowsharp.Server.CodeExamples.Examples.xml");
        if (stream is null)
        {
            throw new InvalidOperationException("Could not find embedded resource 'Lowsharp.Server.Examples.xml'.");
        }

        var serializer = new XmlSerializer(typeof(ExamplesRoot));

        if (serializer.Deserialize(stream) is not ExamplesRoot root)
        {
            throw new InvalidOperationException("Could not deserialize embedded resource 'Lowsharp.Server.Examples.xml'.");
        }

        _examples = root.Examples;
    }

    public IEnumerator<Example> GetEnumerator()
    {
        foreach (var example in _examples)
        {
            yield return example;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
