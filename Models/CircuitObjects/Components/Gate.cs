using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components;


public abstract class Gate : Component
{
    public Terminal Output = null!;
}
