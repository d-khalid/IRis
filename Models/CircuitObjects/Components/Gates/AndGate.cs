using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components.Gates;

public class AndGate : MultiInputGate
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
            if (i.State == LogicState.Low)
            {
                Output.State = LogicState.Low;
                return;
            }
        }

        Output.State = LogicState.High;
    }

    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
