using IRis.Models.Base;


namespace IRis.Models.Main.Canvas;


public abstract class CircuitObject : ISimulatable
{
    public abstract void Simulate();
    public abstract void Reset();
}
