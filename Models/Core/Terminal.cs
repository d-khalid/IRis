using Avalonia;
using IRis.Models.Core;


namespace IRis.Models.Core;


public enum TerminalType
{
    Input,
    Output
}


public class Terminal(TerminalType type)
{
    public LogicState State = LogicState.Unknown;
    public readonly TerminalType Type = type;
}

