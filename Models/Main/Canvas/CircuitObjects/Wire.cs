using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects;


public class Wire : CircuitObject
{
    public Terminal MainInput = new();
    public Terminal MainOutput = new();


    public override void Simulate()
    {
        MainOutput.State = MainInput.State;
    }


    public override void Reset()
    {
        MainOutput.State = LogicState.Unknown;
    }
}
