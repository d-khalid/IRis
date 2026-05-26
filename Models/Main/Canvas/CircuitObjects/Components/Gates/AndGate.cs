using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


public class AndGate(Terminal i1, Terminal i2, Terminal output) : MultiInputGate(i1, i2, output)
{
    public override void Simulate()
    {
        LogicState result = LogicState.High;
        foreach (var i in Inputs)
        {
            if (i.State == LogicState.Low)
            {
                result = LogicState.Low;
                break;
            }
        }

        Output.State = result;
    }
}
