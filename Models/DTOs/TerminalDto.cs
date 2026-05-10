using Avalonia;
using IRis.Models.Components;
using IRis.Models.Core;

namespace IRis.Models.DTOs;

public class TerminalDto
{
    public Point Position { get; set; }
    public bool IsOutputProvider { get; set; }

    public static TerminalDto ToDto(Terminal terminal)
    {
        return new TerminalDto
        {
            Position = terminal.Position,
            IsOutputProvider = terminal.isOutputProvider,
        };
    }

    public static Terminal ToTerminal(TerminalDto dto)
    {
        return new Terminal(dto.Position, dto.IsOutputProvider);
    }
}
