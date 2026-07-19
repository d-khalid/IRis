using CommunityToolkit.Mvvm.ComponentModel;

namespace IRis.Models;

public abstract class CircuitObject : ObservableObject, ISimulatable
{
    public abstract void Simulate();
    public abstract void Reset();
}
