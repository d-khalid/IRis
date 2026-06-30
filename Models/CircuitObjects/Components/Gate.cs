using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public abstract class Gate : Component
{
    public Terminal Output = null!;
}
