using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Avalonia;
using Avalonia.Controls;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Services;

public class XmlSerializationService : ISerializationService
{
    public void SerializeComponents(Simulation simulation, string? filePath)
    {
        if (filePath == null)
        {
            Console.WriteLine("No file selected!");
            return;
        }

        XmlSerializer serializer = new XmlSerializer(typeof(CircuitDto));
        List<ComponentDto> dtoList = simulation.Components.Select(p => p.ToDto()).ToList();
        CircuitDto circuit = new CircuitDto() { Components = dtoList };
        

        StreamWriter writer = new StreamWriter(filePath);
        serializer.Serialize(writer, circuit);

        writer.Close();
    }
    

    public List<Component> DeserializeComponentsAsync(string xmlContent)
    {
        var serializer = new XmlSerializer(typeof(CircuitDto));
    
        using (var reader = new StringReader(xmlContent))
        {
            CircuitDto dto = (CircuitDto)serializer.Deserialize(reader);
        
            // Convert to components
            List<Component> components = dto.Components
                .Select(p => ISerializationService.ConvertDtoToComponent(p))
                .ToList();


            
            // Do post-processing, connect wires
            // If a wire has no points, give it points
            
            Dictionary<Guid, Wire> wireDict = new();
            
            Dictionary<Guid, Wire> pointlessWireDict = new();
            
            
            foreach (Component c in components)
            {
                if (c is Wire wire && wire.Id != null)
                {
                    wireDict.Add((Guid)wire.Id, wire);
            
                    // Add wires with no points to a seperate dictionary
                    if (wire.Points.Count == 0)
                    {
                        pointlessWireDict.Add((Guid)wire.Id, wire);
                    }
            
                }
            }
            
            // Now perform replacements
            foreach (Component c in components)
            {
                if (c.Terminals == null) continue;
            
                foreach (Terminal t in c.Terminals)
                {
                    if(t.Wire == null || t.Wire.Id == null) continue;
                
                    t.Wire = wireDict[(Guid)t.Wire.Id];
            
                    // If the wire had no points,
                    // then add the position of every terminal that references it.
                    if (pointlessWireDict.TryGetValue((Guid)t.Wire.Id, out var pointlessWire))
                    {
                        pointlessWire.AddPoint(t.Position + new Point(Canvas.GetLeft(c), Canvas.GetTop(c)));
                    }
            
                }
            }

            return components;
        }
    }
}