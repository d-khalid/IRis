using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using IRis.Models;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Services;


// Will be implemented by a JSON and XML serialization service
public interface ISerializationService
{
    public void SerializeComponents(Simulation simulation, string fileName);
    public List<Component> DeserializeComponentsAsync(string content);
    async Task<List<Component>> DeserializeFromFileAsync(string filePath)
    {
        string data = await File.ReadAllTextAsync(filePath);
        return DeserializeComponentsAsync(data);
    }

 
    public static Component ConvertDtoToComponent(ComponentDto dto)
    {
        int numInputs = ParseProperty<int>(dto, "NumInputs");
        double width = ParseProperty<double>(dto, "Width");
        double height = ParseProperty<double>(dto, "Height");
        double rotation = ParseProperty<double>(dto, "Rotation");
        
        // Terminal[] terminals = ParseProperty<Terminal[]>(dto, "Terminals");


        Component component = dto.Type switch
        {
            "AndGate" => new AndGate(numInputs),
            "OrGate" => new OrGate(numInputs),
            "NorGate" => new NorGate(numInputs),
            "NandGate" => new NandGate(numInputs),
            "XorGate" => new XorGate(numInputs),
            "XnorGate" => new XnorGate(numInputs),
            "NotGate" => new NotGate(),
            
            "Wire" => new Wire(),
            "LogicToggle" => new LogicToggle(),
            "LogicProbe" => new LogicProbe(),


            _ => throw new NotSupportedException($"Unknown component type: {dto.Type}")
        };

        // Set common properties
        Canvas.SetLeft(component, dto.X);
        Canvas.SetTop(component, dto.Y);
        
        component.Width = width;
        component.Height = height;

        component.Rotation = rotation;

        // Assign wire IDs to terminals, if the component has them
        if (component.Terminals != null)
        {
            for (int i = 0; i < component.Terminals.Length; i++)
            {
                if(dto.Terminals[i].ConnectedWireId != null)
                    component.Terminals[i].Wire = new Wire()
                    {
                        Id = dto.Terminals[i].ConnectedWireId,
                    };
            }
        }
        
        // Set component-specific properties
        switch (component)
        {
            case Wire wire:
                // Upcast
                WireDto wireDto = (WireDto)dto;
                
                // Add wire points and ID
                wire.Points = wireDto.Points.Select(p => p.ToPoint()).ToList();
                wire.Id = wireDto.Id;
                
                break;
            case LogicToggle toggle:
                toggle.Value = ParseProperty<LogicState>(dto, "Value");
                break;
        }

        return component;
    }

    public static T ParseProperty<T>(ComponentDto dto, string name)
    {
        PropertyDto? prop = dto.Properties.FirstOrDefault(p => p.Name == name);
        if (prop == null) return default;
    
        Console.WriteLine($"Property: ({prop.Name}, {prop.Value})");

        Type targetType = typeof(T);
    
        // Special handling for GUIDs
        if (targetType == typeof(Guid))
        {
            if (Guid.TryParse(prop.Value, out Guid guid))
            {
                return (T)(object)guid;
            }
            return default;
        }
    
        // Special handling for nullable GUIDs
        if (targetType == typeof(Guid?))
        {
            if (string.IsNullOrEmpty(prop.Value)) return default;
            if (Guid.TryParse(prop.Value, out Guid guid))
            {
                return (T)(object)guid;
            }
            return default;
        }

        // Special handling for enums
        if (targetType.IsEnum)
        {
            if (Enum.TryParse(targetType, prop.Value, out object result))
            {
                return (T)result;
            }
            return default;
        }
    
        // Special handling for nullable types
        Type underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
        {
            if (string.IsNullOrEmpty(prop.Value)) return default;
            targetType = underlyingType;
        }

        // Default conversion for other types
        try
        {
            return (T)Convert.ChangeType(prop.Value, targetType);
        }
        catch
        {
            return default;
        }
    }

}