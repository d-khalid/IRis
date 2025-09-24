using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models.DTOs;

public class TerminalDto
{
    
    public List<Guid> ConnectedWireIds { get; set; }

    public static TerminalDto ToDto(Terminal t)
    {
        return new TerminalDto()
        {
            ConnectedWireIds = t.Wires.Select(w => w.Id).ToList(),
        };
    }

    // Make dummy wire objects with the reference of the correct object
    // This is required for post processing
    public static Terminal ToTerminal(TerminalDto dto)
    {
        // Placeholder position
        return new Terminal(new Point(1,1))
        {
            Wires = dto.ConnectedWireIds.Select(p => new Wire(){Id = p}).ToList()
        };
    }
}