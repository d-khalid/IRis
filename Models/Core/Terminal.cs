using Avalonia;
using IRis.Models.Components;
using System.Collections.Generic;
using System.Linq;

namespace IRis.Models.Core;

public class Terminal
{
    public Point Position { get; }

    // Change from single Wire to List of Wires
    public List<Wire> Wires { get; set; } = new List<Wire>();

    // Keep this for backward compatibility if needed
    public Wire? Wire
    {
        get => Wires.FirstOrDefault();
        set
        {
            if (value != null && !Wires.Contains(value))
            {
                Wires.Add(value);
            }
        }
    }

    public Terminal(Point position)
    {
        Position = position;
    }

    public Terminal(Point position, Wire wire) : this(position)
    {
        if (wire != null)
            Wires.Add(wire);
    }

    // Add a wire to this terminal
    public void AddWire(Wire wire)
    {
        if (wire != null && !Wires.Contains(wire))
        {
            Wires.Add(wire);
        }
    }

    // Remove a wire from this terminal
    public void RemoveWire(Wire wire)
    {
        Wires.Remove(wire);
    }


}