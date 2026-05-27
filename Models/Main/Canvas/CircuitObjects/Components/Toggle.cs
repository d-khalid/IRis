using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components;


public class Toggle : Component
{
    public Terminal Output = null!;
    public LogicState State = LogicState.Low;


    public override void Simulate()
    {
        Output.State = State;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
