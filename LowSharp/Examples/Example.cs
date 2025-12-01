using System.Xml.Serialization;

namespace LowSharp.Examples;

public class Example
{
    [XmlAttribute("n")]
    public required string Name { get; set; }

    [XmlAttribute("t")]
    public required string Type { get; set; }

    [XmlText()]
    public required string Value { get; set; }
}