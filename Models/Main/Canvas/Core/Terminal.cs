namespace IRis.Models.Main.Canvas.Core;


public class Terminal(TerminalType type)
{
    public TerminalType Type = type;
}


public enum TerminalType
{
    Input,
    Output
}

public enum LogicState
{
    High,
    Low,
    Unknown
}
