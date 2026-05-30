using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components;


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
