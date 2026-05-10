using System.Linq;
using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using System;

using IRis.Models.Core;


namespace  IRis.Models.Components;


public class NorGate(int numInputs = Constants.NorGateDefaultNumInputs) : 
    Gate(numInputs, size: Constants.NorGateSize)
{
    public List<Terminal> Inputs
    {
        get => _inputs;
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override NorGate Clone()
    {
        NorGate clone = new(
            numInputs: Inputs.Count
        );

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry NorGateGeometry = new();
        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        if (NorGateGeometry.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddOrSymbolToFigure(figure, Size);

        NorGateGeometry.Figures.Add(figure);
        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: NorGateGeometry
        );

        Utils.AddNotBubbleToDrawing(ctx, Size);
    }
    

    public override void ComputeOutput()
    {
        if (Inputs.Any(i => i.State == LogicState.Unknown))
        {
            Output.State = LogicState.Unknown;
            return;
        }

        Output.State = (LogicState)(Inputs
            .Select(t => (int)t.State)
            .Aggregate((a, b) => a | b) ^ 1);
    }
}
    
