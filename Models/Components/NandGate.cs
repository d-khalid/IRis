using System.Linq;
using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;

using IRis.Models.Core;


namespace  IRis.Models.Components;


public class NandGate(int numInputs = Constants.NandGateDefaultNumInputs) : 
    Gate(numInputs, size: Constants.NandGateSize)
{
    public List<(Terminal Terminal, Point Position)> Inputs
    {
        get => _inputs;
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override NandGate Clone()
    {
        NandGate clone = new(
            numInputs: Inputs.Count
        );

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry NandGateGeometry = new();
        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        if (NandGateGeometry.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddAndSymbolToFigure(figure, Size);

        NandGateGeometry.Figures.Add(figure);
        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: NandGateGeometry
        );

        Utils.AddNotBubbleToDrawing(ctx, Size);
    }
    

    public override void ComputeOutput()
    {
        if (Inputs.Any(i => i.Terminal.State == LogicState.Unknown))
        {
            Output.Terminal.State = LogicState.Unknown;
            return;
        }

        Output.Terminal.State = (LogicState)(Inputs
            .Select(i => (int)i.Terminal.State)
            .Aggregate((a, b) => a & b) ^ 1);
    }
}

