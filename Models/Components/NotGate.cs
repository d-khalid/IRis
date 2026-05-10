using Avalonia;
using Avalonia.Media;
using System;

using IRis.Models.Core;


namespace  IRis.Models.Components;


public class NotGate() : Gate(numInputs: 1, size: Constants.OrGateSize)
{
    public Terminal Input
    {
        get => _inputs[0];
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override OrGate Clone()
    {
        OrGate clone = new();
        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry NotGateGeometry = new();
        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        if (NotGateGeometry.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddNotSymbolToFigure(figure, Size);

        NotGateGeometry.Figures.Add(figure);
        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: NotGateGeometry
        );

        Utils.AddNotBubbleToDrawing(ctx, Size);
    }
    

    public override void ComputeOutput()
    {
        if (Input.State == LogicState.Unknown)
        {
            Output.State = LogicState.Unknown;
            return;
        }

        Output.State = (LogicState)((int)Input.State ^ 1);
    }
}

