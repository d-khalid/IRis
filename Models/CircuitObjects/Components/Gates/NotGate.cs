using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components.Gates;

public class NotGate : Gate
{
    public Terminal Input = null!;

    public override void Simulate()
    {
        if (Input.State == LogicState.Unknown)
            Output.State = LogicState.Unknown;
        else if (Input.State == LogicState.High)
            Output.State = LogicState.Low;
        else if (Input.State == LogicState.Low)
            Output.State = LogicState.High;
    }

    public override void Reset() => Output.State = LogicState.Unknown;
}
