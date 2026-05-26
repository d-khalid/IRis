namespace IRis.Models.Main.Canvas.Core;


public class Terminal()
{
    public LogicState State = LogicState.Low;
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
