using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects;


public class Wire(Terminal mainInput, Terminal mainOutput) : CircuitObject
{
    public Terminal MainInput = mainInput;
    public Terminal MainOutput = mainOutput;


    public override void Simulate()
    {
        MainOutput.State = MainInput.State;
    }


    public override void Reset()
    {
        MainOutput.State = LogicState.Unknown;
    }
}
