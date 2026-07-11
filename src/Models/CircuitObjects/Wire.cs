using System.Collections.Generic;
using IRis.Models.Core;

namespace IRis.Models.CircuitObjects;

public class Wire : CircuitObject
{
    public Terminal MainInput = null!;
    public Terminal MainOutput = null!;
    public List<Terminal> Outputs { get; set; } = [];

    public override void Simulate()
    {
        if (MainOutput is null || MainInput is null)
            return;
        MainOutput.State = MainInput.State;

        foreach (Terminal output in Outputs)
        {
            output.State = MainInput.State;
        }
    }

    public override void Reset()
    {
        MainOutput.State = LogicState.Unknown;

        foreach (Terminal output in Outputs)
        {
            output.State = LogicState.Unknown;
        }
    }
}
