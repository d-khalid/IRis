using System.Collections.Generic;
using IRis.Models.Core;


namespace IRis.Models.CircuitObjects.Components;


public class Multiplexer : Component
{
    public List<Terminal> Selects { get; } = [];
    public List<Terminal> Inputs { get; } = [];
    public Terminal Output = null!;


    public override void Simulate()
    {
        int index = 0;

        for (int i = 0; i < Selects.Count; i++)
        {
            if (Selects[i].State == LogicState.Unknown)
            {
                Output.State = LogicState.Unknown;
                return;
            }

            if (Selects[i].State == LogicState.High)
                index |= 1 << i;
        }

        if (index >= Inputs.Count)
        {
            Output.State = LogicState.Unknown;
            return;
        }

        Output.State = Inputs[index].State;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
