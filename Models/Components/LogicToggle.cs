using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using System;

using IRis.Models.Core;


namespace IRis.Models.Components;


public class LogicToggle() : 
    Component(numInputs: 0, numOutputs: 1, size: Constants.LogicToggleSize), IOutputProvider
{
    public LogicState State = LogicState.Low;


    public (Terminal Terminal, Point Position) Output
    {
        get => _outputs[0];
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override LogicToggle Clone()
    {
        LogicToggle clone = new()
        {
            State = State
        };

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawOutputTerminal(ctx);

        ImmutableSolidColorBrush brush = State switch {
            LogicState.High => Constants.TrueStateBrush,
            LogicState.Low => Constants.FalseStateBrush,
            _ => Constants.UnknownStateBrush
        };

        ctx.DrawRectangle(
            brush: brush,
            pen: Constants.LogicTogglePen,
            rect: new Rect(0, 0, Size.Width, Size.Height)
        );

        Utils.AddBigTextToDrawing(
            ctx: ctx,
            position: new Point(
                (Size.Width - Constants.DrawingBigTextSize * 0.5) / 2, 
                (Size.Height - Constants.DrawingBigTextSize) / 2
            ),
            text: State == LogicState.Unknown ? "X" : ((int)State).ToString()
        );
    }


    private void DrawOutputTerminal(DrawingContext ctx)
    {
        ctx.DrawLine(
            pen: Constants.TerminalWirePen,
            p2: new Point(Output.Position.X - Constants.TerminalWireLength, Output.Position.Y),
            p1: Output.Position
        );

        ctx.DrawEllipse(
            brush: Constants.TerminalBubbleBrush,
            pen: null,
            center: Output.Position, 
            radiusX: Constants.TerminalBubbleRadius,
            radiusY: Constants.TerminalBubbleRadius
        );
    }


    public void ComputeOutput()
    {
        Output.Terminal.State = State;
    }
}

