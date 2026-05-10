using Avalonia;
using Avalonia.Media;
using System.Linq;
using System.Collections.Generic;
using System;

using IRis.Models.Core;


namespace  IRis.Models.Components;


public class OrGate(int numInputs = Constants.OrGateDefaultNumInputs) : 
    Gate(numInputs, size: Constants.OrGateSize)
{
    public List<Terminal> Inputs
    {
        get => _inputs;
    }


    // public override void Serialize()
    // {
    //     throw new NotImplementedException();
    // }


    public override OrGate Clone()
    {
        OrGate clone = new(
            numInputs: Inputs.Count
        );

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry OrGateGeometry = new();
        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        if (OrGateGeometry.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddOrSymbolToFigure(figure, Size);

        OrGateGeometry.Figures.Add(figure);
        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: OrGateGeometry
        );
    }
    

    public override void ComputeOutput()
    {
        if (Inputs.Any(i => i.State == LogicState.Unknown))
        {
            Output.State = LogicState.Unknown;
            return;
        }

        Output.State = (LogicState)Inputs
            .Select(t => (int)t.State)
            .Aggregate((a, b) => a | b);
    }
}

