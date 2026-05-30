using System.Collections.Generic;
using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components.Gates;


public abstract class MultiInputGate : Gate
{
    public List<Terminal> Inputs { get; } = [];
}
