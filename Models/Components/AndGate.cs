using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;


namespace  IRis.Models.Components;


public class AndGate(int numInputs = Constants.AndGateDefaultNumInputs) : 
    Gate(numInputs, size: Constants.AndGateSize)
{
    public List<Terminal> Inputs
    {
        get => _inputs;
    }


    public override void Serialize()
    {
        throw new NotImplementedException();
    }


    public override AndGate Clone()
    {
        AndGate clone = new(
            numInputs: Inputs.Count
        );

        return clone;
    }


    public override void Draw(DrawingContext ctx)
    {
        DrawTerminals(ctx);

        PathGeometry AndGateGeometry = new();
        PathFigure figure = new()
        {
            StartPoint = new Point(0, 0),
            IsClosed = true
        };

        if (AndGateGeometry.Figures == null)
            throw new Exception("Cannot draw: PathGeometry.Figures is null.");

        Utils.AddAndSymbolToFigure(figure, Size);

        AndGateGeometry.Figures.Add(figure);
        ctx.DrawGeometry(
            brush: Constants.GateBrush, 
            pen: Constants.GatePen, 
            geometry: AndGateGeometry
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
            .Aggregate((a, b) => a & b);
    }
}

