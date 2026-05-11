using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models.DTOs;

public class CircuitDto
{
    public List<ComponentDto> Components {get; set;}
    public List<WireDto> Wires {get; set;}

    public static List<Component> ToCircuit(CircuitDto circuit)
    {
        List<Wire> wires = circuit.Wires
            .Select(p => WireDto.ToWire(p))
            .ToList();
        
        List<Component> components = circuit.Components
            .Select(p => ComponentDto.ToComponent(p))
            .Where(p => p != null)
            .ToList();
        
        // Make a dictionary of wires for fast lookups
        Dictionary<Guid, Wire> wireDict = new Dictionary<Guid, Wire>();
        foreach (var wire in wires)
        {
            if (!wireDict.ContainsKey(wire.Id))
            {
                wireDict.Add(wire.Id, wire);
            }
        }
        
        // Assign wire references to terminals based on ID
        foreach (var c in components)
        {
            if (c.Terminals == null) continue;

            foreach (var terminal in c.Terminals)
            {
                if (terminal.Wires == null)
                {
                    terminal.Wires = new List<Wire>();
                    continue;
                }

                terminal.Wires = terminal.Wires
                    .Select(w => wireDict.TryGetValue(w.Id, out var mapped) ? mapped : null)
                    .Where(w => w != null)
                    .ToList()!;
            }
        }
        
        
        
        // Pool them together
        components.AddRange(wires);
        return components;
    }
}