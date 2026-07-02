using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components.Gates;

public class XorGate : MultiInputGate
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

        int highs = 0;

        foreach (var i in Inputs)
        {
            if (i.State == LogicState.High)
                highs++;
        }

        Output.State = highs % 2 == 1 ? LogicState.High : LogicState.Low;
    }

    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
