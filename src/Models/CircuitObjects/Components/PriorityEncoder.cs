using System.Collections.Generic;
using IRis.Models.Core;
using System;

namespace IRis.Models.CircuitObjects.Components;

public class PriorityEncoder : Component
{
    public List<Terminal> Inputs { get; } = [];
    public List<Terminal> Outputs { get; } = [];

    public override void Simulate()
    {
        for (int i = 0; i < Inputs.Count; i++)
        {
            if (Inputs[i].State == LogicState.Unknown)
            {
                foreach (var o in Outputs)
                    o.State = LogicState.Unknown;
                return;
            }
        }

        int index = -1;

        for (int i = Inputs.Count - 1; i >= 0; i--)
        {
            if (Inputs[i].State == LogicState.High)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            foreach (var o in Outputs)
                o.State = LogicState.Low;
            return;
        }

        for (int bit = 0; bit < Outputs.Count; bit++)
        {
            Outputs[bit].State = index / (int)Math.Pow(2, bit) % 2 == 1
                ? LogicState.High
                : LogicState.Low;
        }
    }

    public override void Reset()
    {
        foreach (var o in Outputs)
            o.State = LogicState.Unknown;
    }
}
