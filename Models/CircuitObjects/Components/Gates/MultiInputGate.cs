using System.Collections.Generic;
using IRis.Models.Core;


namespace IRis.Models.CircuitObjects.Components.Gates;


public abstract class MultiInputGate : Gate
{
    public List<Terminal> Inputs { get; } = [];
}
