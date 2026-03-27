using Avalonia.Controls;
using IRis.Models.Components;
using IRis.Models.Core;
using System.Collections.Generic;
using System.Linq;

namespace IRis.Models.DTOs;

public class ComponentDto
{
    public string Type { get; set; }
    public List<TerminalDto> Terminals { get; set; }

    public int InputLineCount { get; set; }
    public int SelectionLineCount { get; set; }

    public double X { get; set; }
    public double Y { get; set; }

    public Dictionary<string, LogicState> StoredStates { get; set; }

    public double Rotation { get; set; }
    public bool IsSelected { get; set; }

    public static ComponentDto ToDto(CircuitComponent c)
    {
        return new ComponentDto()
        {
            // Get only the type name, without the full namespace
            Type = c.ToString().Split(".")[^1],

            InputLineCount = c.InputLineCount,
            SelectionLineCount = c.SelectionLineCount,
            Rotation = c.Rotation,
            IsSelected = c.IsSelected,

            X = Canvas.GetLeft(c),
            Y = Canvas.GetTop(c),

            StoredStates = c.StoredStates,
            Terminals = c.Terminals.Select(p => TerminalDto.ToDto(p)).ToList(),

        };
    }


    public static CircuitComponent? ToComponent(ComponentDto dto)
    {
        // Pattern matching to ensure that the relevant constructor is always called
        CircuitComponent? result = dto.Type switch
        {
            "AndGate" => new AndGate(dto.InputLineCount),
            "OrGate" => new OrGate(dto.InputLineCount),
            "NotGate" => new NotGate(),
            "XorGate" => new XorGate(dto.InputLineCount),
            "NandGate" => new NandGate(dto.InputLineCount),
            "NorGate" => new NorGate(dto.InputLineCount),
            "XnorGate" => new XnorGate(dto.InputLineCount),

            "LogicToggle" => new LogicToggle(),
            "LogicProbe" => new LogicProbe(),

            "Multiplexer" => new Multiplexer(dto.SelectionLineCount),
            "Demultiplexer" => new Demultiplexer(dto.SelectionLineCount),

            "Encoder" => new Encoder(dto.SelectionLineCount),
            "Decoder" => new Decoder(dto.SelectionLineCount),

            "DLatch" => new DLatch(),
            "JKLatch" => new JKLatch(),
            "SRLatch" => new SRLatch(),
            "TLatch" => new TLatch(),

            _ => null,
        };
        if (result == null) return null;

        result.Rotation = dto.Rotation;
        result.IsSelected = dto.IsSelected;
        result.StoredStates = dto.StoredStates;

        // Add terminal positions here by calling the function that generates them
        result.AddTerminalPoints();
        for (int i = 0; i < dto.Terminals.Count; i++)
        {
            result.Terminals[i].Wires = TerminalDto.ToTerminal(dto.Terminals[i]).Wires;
        }

        Canvas.SetLeft(result, dto.X);
        Canvas.SetTop(result, dto.Y);

        return result;

    }

}