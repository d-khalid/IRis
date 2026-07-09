using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class DLatch : Component
{
    public Terminal D = null!;
    public Terminal En = null!;
    public Terminal Q = null!;
    public Terminal QBar = null!;

    public override void Simulate()
    {
        if (D.State == LogicState.Unknown || En.State == LogicState.Unknown)
        {
            Q.State = LogicState.Unknown;
            QBar.State = LogicState.Unknown;
            return;
        }

        if (En.State == LogicState.High)
        {
            Q.State = D.State;
            QBar.State = D.State == LogicState.High ? LogicState.Low : LogicState.High;
        }
    }

    public override void Reset()
    {
        Q.State = LogicState.Unknown;
        QBar.State = LogicState.Unknown;
    }
}
