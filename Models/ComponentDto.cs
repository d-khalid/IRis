using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Avalonia;
using IRis.Models.Core;

namespace IRis.Models;

/*
SERIALIZATION STRATEGY (GUIDs are unique identifiers for .NET objects)
- store GUIDs for Terminals and Wires
- Serialize the circuit with GUIDs
- When deserialzing, reconstruct the terminals and wires using the GUIDs
 */
[Serializable]
[XmlRoot("Component")]
public class ComponentDto
{
    [XmlAttribute("Type")] public string Type { get; set; }

    [XmlAttribute("X")] public double X { get; set; }

    [XmlAttribute("Y")] public double Y { get; set; }

    [XmlElement("Terminals")] public List<TerminalDto> Terminals { get; set; } = new();

    // Add other serializable properties
    [XmlElement("Properties")] public List<PropertyDto> Properties { get; set; } = new();
}

[Serializable]
public class PropertyDto
{
    [XmlAttribute("Name")] public string Name { get; set; }

    [XmlText] public string Value { get; set; }
}


[Serializable]
public class TerminalDto
{
    
    [XmlAttribute("Id")] public Guid Id;
    [XmlAttribute("ConnectedWireId")] public Guid? ConnectedWireId; // Store only the GUID
}

[Serializable]
public class WireDto
{
    [XmlAttribute("Id")] public Guid Id;
    [XmlAttribute("ConnectedWireState")] public LogicState State;
    [XmlElement("ConnectedWireId")] public List<Point> Points = new();

}

