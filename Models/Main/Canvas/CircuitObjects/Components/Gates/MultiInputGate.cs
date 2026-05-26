using System.Collections.Generic;
using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


public abstract class MultiInputGate : Gate
{
    public List<Terminal> Inputs = [];


    public MultiInputGate(Terminal i1, Terminal i2, Terminal output) : base(output)
    {
        AddInput(i1);
        AddInput(i2);
    }


    public void AddInput(Terminal input)
    {
        Inputs.Add(input);
    }


    public void RemoveInput(Terminal input)
    {
        Inputs.Remove(input);
    }
}
