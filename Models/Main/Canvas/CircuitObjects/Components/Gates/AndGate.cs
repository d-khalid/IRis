using System;
using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


public class AndGate() : MultiInputGate(new Terminal(), new Terminal())
{
    public override void Simulate()
    {
        LogicState result = LogicState.High;
        foreach (var i in Inputs)
        {
            if (i.State == LogicState.Low)
                result = LogicState.Low;

            // else if (i.State == LogicState.Unknown)
            //     result = LogicState.Unknown;
        }

        Output.State = result;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
