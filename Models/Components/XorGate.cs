using System.Linq;
using Avalonia;
using Avalonia.Media;
using System.Collections.Generic;
using System;

using IRis.Models.Core;


namespace IRis.Models.Components;


public class XorGate(int numInputs = Constants.XorGateDefaultNumInputs) : 
    Gate(numInputs, size: Constants.XorGateSize)
{


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override XorGate Clone()
    {
        XorGate clone = new(
            numInputs: Inputs.Count
        );

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry XorGateGeometry = new();
        PathGeometry XorGateGeometry2 = new();

        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        PathFigure figure2 = new()
        {
            StartPoint = new Point(-Constants.XorArcDistance, Size.Height*0.02),
            IsClosed = false
        };

        if (XorGateGeometry.Figures == null || XorGateGeometry2.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddOrSymbolToFigure(figure, Size);
        Utils.AddXorCurveToFigure(figure2, Size);

        XorGateGeometry.Figures.Add(figure);
        XorGateGeometry2.Figures.Add(figure2);

        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: XorGateGeometry
        );

        ctx.DrawGeometry(
            brush: null, 
            pen: Constants.GatePen, 
            geometry: XorGateGeometry2
        );
    }
    

    public override void ComputeOutput()
    {
        if (Inputs.Any(i => i.Terminal.State == LogicState.Unknown))
        {
            Output.Terminal.State = LogicState.Unknown;
            return;
        }

        Output.Terminal.State = (LogicState)Inputs
            .Select(i => (int)i.Terminal.State)
            .Aggregate((a, b) => a ^ b);
    }
}

