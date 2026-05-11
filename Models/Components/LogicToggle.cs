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
        get => Outputs[0];
    }


    public void Toggle()
    {
        State = (LogicState)((int)State ^ 1);
        InvalidateVisual();
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override LogicToggle Clone()
    {
        LogicToggle clone = new();
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

        Output.Terminal.Draw(ctx, Output.Position);
    }


    public void ComputeOutput()
    {
        if (Output.Terminal.State != State)
        {
            Output.Terminal.State = State;
            InvalidateVisual();
        }
    }
}

