using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models.DTOs;

public class TerminalDto
{
    public Point Position { get; set; }
    
    public List<Guid> ConnectedWireIds { get; set; }

    public static TerminalDto ToDto(Terminal t)
    {
        return new TerminalDto()
        {
            Position = t.Position,
            ConnectedWireIds = t.Wires.Select(w => w.Id).ToList(),
        };
    }

    // Make dummy wire objects with the reference of the correct object
    // This is required for post processing
    public static Terminal ToTerminal(TerminalDto dto)
    {
        return new Terminal(dto.Position)
        {
            Wires = (dto.ConnectedWireIds ?? new List<Guid>())
                .Select(p => new Wire(){Id = p})
                .ToList()
        };
    }
}