using IRis.Models.Core;


namespace IRis.Models.CircuitObjects.Components;


public class Probe : Component
{
    public Terminal Input = null!;
    public LogicState State = LogicState.Unknown;


    public override void Simulate()
    {
        State = Input.State;
    }


    public override void Reset()
    {
        State = LogicState.Unknown;
    }
}
