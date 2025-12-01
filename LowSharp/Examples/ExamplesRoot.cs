using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace LowSharp.Examples;

public class ExamplesRoot
{
    [XmlElement("e")]
    public required Example[] Examples { get; set; }
}