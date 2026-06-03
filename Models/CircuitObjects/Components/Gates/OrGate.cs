using IRis.Models.Core;


namespace IRis.Models.CircuitObjects.Components.Gates;


public class OrGate : MultiInputGate
{
    public override void Simulate()
    {
        foreach (var i in Inputs)
        {
            if (i.State == LogicState.Unknown)
            {
                Output.State = LogicState.Unknown;
                return;
            }
        }

        foreach (var i in Inputs)
        {
            if (i.State == LogicState.High)
            {
                Output.State = LogicState.High;
                return;
            }
        }

        Output.State = LogicState.Low;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
