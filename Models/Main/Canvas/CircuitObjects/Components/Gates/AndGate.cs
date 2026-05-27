using System;
using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


public class AndGate : MultiInputGate
{
    public override void Simulate()
    {
        LogicState result = LogicState.Unknown;
        foreach (var i in Inputs)
        {
            if (i.State == LogicState.Low)
            {
                result = LogicState.Low;
                break;
            }

            else if (i.State == LogicState.High)
                result = LogicState.High;
        }

        Output.State = result;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
