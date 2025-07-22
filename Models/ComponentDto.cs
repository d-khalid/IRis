using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Avalonia;

[Serializable]
[XmlRoot("Circuit")]  // Changed from "Component" to avoid confusion
public class CircuitDto
{
    [XmlArray("Components")]
    [XmlArrayItem("Component", Type = typeof(ComponentDto))]
    [XmlArrayItem("Wire", Type = typeof(WireDto))]  // Explicitly include WireDto
    public List<ComponentDto> Components { get; set; } = new();
}

[Serializable]
public class ComponentDto
{
    [XmlAttribute("Type")] 
    public string Type { get; set; }

    [XmlAttribute("X")] 
    public double X { get; set; }

    [XmlAttribute("Y")] 
    public double Y { get; set; }

    [XmlArray("Terminals")]
    [XmlArrayItem("Terminal")]
    public List<TerminalDto> Terminals { get; set; } = new();

    [XmlArray("Properties")]
    [XmlArrayItem("Property")]
    public List<PropertyDto> Properties { get; set; } = new();
}

[Serializable]
public class TerminalDto
{
    [XmlElement(IsNullable = true)]
    public Guid? ConnectedWireId { get; set; }
}

[Serializable]
public class PropertyDto
{
    [XmlAttribute("Name")] public string Name { get; set; }

    [XmlText] public string Value { get; set; }
}


[Serializable]
public class WireDto : ComponentDto
{
    [XmlArray("Points")]
    [XmlArrayItem("Point")]
    public List<PointDto> Points { get; set; } = new();
    
    
    [XmlElement(IsNullable = true)]
    public Guid? Id { get; set; }
   
}

[Serializable]
public class PointDto
{
    [XmlAttribute("X")] public double X { get; set; }
    [XmlAttribute("Y")] public double Y { get; set; }

    public Point ToPoint()
    {
        return new Point(X, Y);
    }
}