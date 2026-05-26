using IRis.Models.Main.Canvas.Core;


namespace IRis.Models.Main.Canvas.CircuitObjects.Components;


public abstract class Gate(Terminal output) : Component
{
    public Terminal Output = output;
}
