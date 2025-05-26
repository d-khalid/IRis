using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace IRis.Models;

[Serializable]
[XmlRoot("Component")]
public class ComponentDto
{
    [XmlAttribute("Type")] public string Type { get; set; }

    [XmlAttribute("X")] public double X { get; set; }

    [XmlAttribute("Y")] public double Y { get; set; }

    // Add other serializable properties
    [XmlElement("Properties")] public List<PropertyDto> Properties { get; set; } = new();
}

[Serializable]
public class PropertyDto
{
    [XmlAttribute("Name")] public string Name { get; set; }

    [XmlText] public string Value { get; set; }
}