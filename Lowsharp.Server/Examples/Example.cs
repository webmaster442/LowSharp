using System.Xml.Serialization;

namespace Lowsharp.Server.Examples;

[XmlType(AnonymousType = true)]
public class Example
{
    [XmlAttribute(AttributeName = "name")]
    public required string Name { get; set; }

    [XmlAttribute(AttributeName = "lang")]
    public required string Lang { get; set; }

    [XmlText()]
    public required string Value { get; set; }
}
