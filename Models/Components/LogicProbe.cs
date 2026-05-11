using Avalonia;
using Avalonia.Media;
using System;

using IRis.Models.Core;
using Avalonia.Media.Immutable;


namespace IRis.Models.Components;


public class LogicProbe() : 
    Component(numInputs: 1, numOutputs: 0, size: Constants.LogicProbeSize), IOutputProvider
{
    public LogicState State = LogicState.Unknown;


    public (Terminal Terminal, Point Position) Input
    {
        get => _inputs[0];
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override LogicProbe Clone()
    {
        LogicProbe clone = new();
        return clone;
    }
    

    public override void Draw(DrawingContext ctx)
    {
        DrawInputTerminal(ctx);

        ImmutableSolidColorBrush brush = State switch {
            LogicState.High => Constants.TrueStateBrush,
            LogicState.Low => Constants.FalseStateBrush,
            _ => Constants.UnknownStateBrush
        };

        ctx.DrawEllipse(
            brush: brush,
            pen: Constants.LogicProbePen,
            center: new Point(Size.Width / 2, Size.Height / 2),
            radiusX: Size.Width / 2, 
            radiusY: Size.Height / 2
        );

        Utils.AddBigTextToDrawing(
            ctx: ctx, 
            position: new Point(
                (Size.Width - Constants.DrawingBigTextSize * 0.75) / 2, 
                (Size.Height - Constants.DrawingBigTextSize) / 2
            ), 
            text: State == LogicState.Unknown ? "X" : ((int)State).ToString()
        );
    }


    private void DrawInputTerminal(DrawingContext ctx)
    {
        ctx.DrawLine(
            pen: Constants.TerminalWirePen, 
            p1: Input.Position, 
            p2: new Point(Constants.TerminalWireLength, Input.Position.Y)
        );

        Input.Terminal.Draw(ctx, Input.Position);
    }


    public void ComputeOutput()
    {
        if (Input.Terminal.State != State)
        {
            State = Input.Terminal.State;
            InvalidateVisual();
        }
    }
}

