using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class Clock : Component
{
    public Terminal Output = null!;
    public LogicState State = LogicState.Low;
    public double FrequencyHz = 1.0;

    public override void Simulate()
    {
        Output.State = State;
    }

    public override void Reset()
    {
        State = LogicState.Unknown;
        Output.State = LogicState.Unknown;
    }
}
