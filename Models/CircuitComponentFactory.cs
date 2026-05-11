using System;
using System.Collections.Generic;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models;

internal static class CircuitComponentFactory
{
    private static readonly IReadOnlyDictionary<string, Func<Component?>> Factories =
        new Dictionary<string, Func<Component?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["AND"] = () => new AndGate(),
            ["ANDGATE"] = () => new AndGate(),
            ["OR"] = () => new OrGate(),
            ["ORGATE"] = () => new OrGate(),
            ["NOT"] = () => new NotGate(),
            ["NOTGATE"] = () => new NotGate(),
            ["NAND"] = () => new NandGate(),
            ["NANDGATE"] = () => new NandGate(),
            ["NOR"] = () => new NorGate(),
            ["NORGATE"] = () => new NorGate(),
            ["XOR"] = () => new XorGate(),
            ["XORGATE"] = () => new XorGate(),
            ["XNOR"] = () => new XnorGate(),
            ["XNORGATE"] = () => new XnorGate(),
            ["TOGGLE"] = () => new LogicToggle(),
            ["LOGICTOGGLE"] = () => new LogicToggle(),
            ["PROBE"] = () => new LogicProbe(),
            ["LOGICPROBE"] = () => new LogicProbe(),
            ["DLATCH"] = () => new DLatch(),
        };

    private static readonly HashSet<string> SupportedTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "AND", "ANDGATE",
        "OR", "ORGATE",
        "NOT", "NOTGATE",
        "NAND", "NANDGATE",
        "NOR", "NORGATE",
        "XOR", "XORGATE",
        "XNOR", "XNORGATE",
        "TOGGLE", "LOGICTOGGLE",
        "PROBE", "LOGICPROBE",
        "DLATCH", 
    };

    public static bool IsSupportedComponentType(string? componentType)
    {
        return !string.IsNullOrWhiteSpace(componentType) && SupportedTypes.Contains(componentType.Trim());
    }

    public static Component? Create(string? componentType)
    {
        if (string.IsNullOrWhiteSpace(componentType))
            return null;

        return Factories.TryGetValue(componentType.Trim(), out var factory)
            ? factory()
            : null;
    }

    public static Component? CloneComponent(Component component)
    {
        Component? clone = component switch
        {
            AndGate gate => new AndGate(gate.Inputs.Count),
            OrGate gate => new OrGate(gate.Inputs.Count),
            NotGate => new NotGate(),
            NandGate gate => new NandGate(gate.Inputs.Count),
            NorGate gate => new NorGate(gate.Inputs.Count),
            XorGate gate => new XorGate(gate.Inputs.Count),
            XnorGate gate => new XnorGate(gate.Inputs.Count),
            LogicToggle toggle => new LogicToggle { State = toggle.State },
            LogicProbe probe => new LogicProbe { State = probe.State },
            DLatch latch => new DLatch(),
            _ => null,
            
        };

        if (clone != null)
            clone.Orientation = component.Orientation;

        return clone;
    }

    public static int GetInputCount(Component component)
    {
        
        return component.Inputs.Count;
        
        // why even?
        // return component switch
        // {
        //     AndGate gate => gate.Inputs.Count,
        //     OrGate gate => gate.Inputs.Count,
        //     NotGate => 1,
        //     NandGate gate => gate.Inputs.Count,
        //     NorGate gate => gate.Inputs.Count,
        //     XorGate gate => gate.Inputs.Count,
        //     XnorGate gate => gate.Inputs.Count,
        //     DLatch latch => latch.Inputs.Count,
        //     _ => 0,
        // };
    }

    public static string GetComponentTypeName(Component component)
    {
        return component.GetType().Name;
    }
}