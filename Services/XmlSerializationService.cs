// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Xml;
// using System.Xml.Serialization;
// using Avalonia;
// using Avalonia.Controls;
// using IRis.Models;
// using IRis.Models.Components;
// using IRis.Models.Core;
//
// namespace IRis.Services;
//
// public class XmlSerializationService : ISerializationService
// {
//     public void SerializeComponents(Simulation simulation, string? filePath)
//     {
//         if (filePath == null)
//         {
//             Console.WriteLine("No file selected!");
//             return;
//         }
//
//         XmlSerializer serializer = new XmlSerializer(typeof(CircuitDto));
//         List<ComponentDto> dtoList = simulation.Components.Select(p => p.ToDto()).ToList();
//         CircuitDto circuit = new CircuitDto() { Components = dtoList };
//
//
//         StreamWriter writer = new StreamWriter(filePath);
//         serializer.Serialize(writer, circuit);
//
//         writer.Close();
//     }
//
//
//     public List<Component> DeserializeComponentsAsync(string xmlContent)
//     {
//         var serializer = new XmlSerializer(typeof(CircuitDto));
//
//         using (var reader = new StringReader(xmlContent))
//         {
//             CircuitDto dto = (CircuitDto)serializer.Deserialize(reader)!;
//             // Convert to components
//             List<Component> components = dto.Components
//                 .Select(p => ISerializationService.ConvertDtoToComponent(p))
//                 .ToList();
//
//             // Connect the wires to components
//             foreach (Component thisComponent in components)
//             {
//                 if (thisComponent is Wire || thisComponent.Terminals == null) continue;
//                 foreach (Terminal terminal in thisComponent.Terminals)
//                 {
//                     List<Wire> connectedWires = GetConnectedWires(components, thisComponent, terminal);
//                     if (connectedWires == null) break;
//                     // Add connected wires to terminals
//                     foreach (Wire wire in connectedWires)
//                     {
//                         terminal.AddWire(wire);
//                     }
//                 }
//             }
//             return components;
//         }
//     }
//
//     private static List<Wire> GetConnectedWires(List<Component> components, Component thisComponent, Terminal terminal)
//     {
//         // Get absolute position of the terminal
//         Point absolutePosition = new(
//                         terminal.Position.X + Canvas.GetLeft(thisComponent),
//                         terminal.Position.Y + Canvas.GetTop(thisComponent)
//                     );
//         List<Wire> connectedWires = []; // Initialize an empty list
//
//         foreach (Component component in components)
//         {
//             if (component is not Wire wire) continue;   // We're looking for wires
//
//             foreach (Point point in wire.Points)
//             {
//                 if (point == absolutePosition)  // If the wire point is exactly on the terminal
//                 {
//                     connectedWires.Add(wire);
//                 }
//             }
//         }
//         return connectedWires;
//     }
// }