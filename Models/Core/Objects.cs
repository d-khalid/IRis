using Avalonia;
using Avalonia.Media;
using Avalonia.Controls;
using Avalonia.Media.Immutable;
using System;


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


public class Terminal(Point position, bool isOutputProvider = false) : Control
{
    public readonly Point Position = position;
    public LogicState State = LogicState.Unknown;
    public readonly bool IsOutputProvider = isOutputProvider;


    public void Draw(DrawingContext ctx)
    {
        ImmutableSolidColorBrush brush = State switch {
            LogicState.High => Constants.TrueStateBrush,
            LogicState.Low => Constants.FalseStateBrush,
            _ => Constants.UnknownStateBrush
        };

        ctx.DrawEllipse(
            brush: brush,
            pen: null,
            center: Position,
            radiusX: Constants.TerminalBubbleRadius,
            radiusY: Constants.TerminalBubbleRadius
        );
    }
}


public enum LogicState
{
    High = 1,
    Low = 0,
    Unknown = -1
}

