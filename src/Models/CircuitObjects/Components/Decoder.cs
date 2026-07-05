using System;
using System.Collections.Generic;
using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class Decoder : Component
{
    public List<Terminal> Selects { get; } = [];
    public List<Terminal> Outputs { get; } = [];

    public override void Simulate()
    {
        int index = 0;

        for (int i = 0; i < Selects.Count; i++)
        {
            if (Selects[i].State == LogicState.Unknown)
            {
                foreach (var o in Outputs)
                    o.State = LogicState.Unknown;
                return;
            }

            if (Selects[i].State == LogicState.High)
                index += (int)Math.Pow(2, i);
        }

        if (index >= Outputs.Count)
        {
            foreach (var o in Outputs)
                o.State = LogicState.Unknown;
            return;
        }

        for (int i = 0; i < Outputs.Count; i++)
            Outputs[i].State = i == index ? LogicState.High : LogicState.Low;
    }

    public override void Reset()
    {
        foreach (var o in Outputs)
            o.State = LogicState.Unknown;
    }
}
