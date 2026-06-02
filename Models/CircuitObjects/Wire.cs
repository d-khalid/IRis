using IRis.Models.Core;


namespace IRis.Models.CircuitObjects;


public class Wire : CircuitObject
{
    public Terminal MainInput = null!;
    public Terminal MainOutput = null!;


    public override void Simulate()
    {
        if (MainOutput is null || MainInput is null) return;
        MainOutput.State = MainInput.State;
    }


    public override void Reset()
    {
        MainOutput.State = LogicState.Unknown;
    }
}
