namespace IRis.Models;


public abstract class CircuitObject : ISimulatable
{
    public abstract void Simulate();
    public abstract void Reset();
}
