// default libs
using Avalonia;


namespace IRis.Models.Core;


public enum ComponentOrientation
{
    Right = 0,
    Down = 90,
    Left = 180,
    Up = 270
}


public struct BoxSize(int width, int height)
{
    public int Width = width;
    public int Height = height;
}


public class Terminal(Point position, bool isOutputProvider)
{
    public Point Position = position;
    public readonly bool isOutputProvider = isOutputProvider;
    public LogicState State = LogicState.Unknown;
}


public enum LogicState
{
    High = 1,
    Low = 0,
    Unknown = -1
}

