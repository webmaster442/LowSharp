using System.Xml.Serialization;

namespace Lowsharp.Server.CodeExamples;

[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", ElementName ="examples", IsNullable = false)]
public class ExamplesRoot
{
    [XmlElement("e")]
    public required Example[] Examples { get; set; }
}

