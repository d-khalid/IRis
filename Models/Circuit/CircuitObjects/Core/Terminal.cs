namespace IRis.Models.Circuit.CircuitObjects.Core;


public class Terminal(TerminalType type)
{
    public TerminalType Type = type;
}


public enum TerminalType
{
    Input,
    Output
}
