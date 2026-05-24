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
        DrawTerminals(ctx, notBubbleMode: true);

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
        if (Inputs.Any(i => i.Terminal.State == LogicState.Unknown))
        {
            Output.Terminal.State = LogicState.Unknown;
            return;
        }

        Output.Terminal.State = (LogicState)(Inputs
            .Select(i => (int)i.Terminal.State)
            .Aggregate((a, b) => a | b) ^ 1);
    }
}
    
