using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components;


public class Toggle : Component
{
    public Terminal Output = new();
    public LogicState State = LogicState.High;


    public override void Simulate()
    {
        Output.State = State;
    }


    public override void Reset()
    {
        Output.State = LogicState.Unknown;
    }
}
