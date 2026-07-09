using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class DFlipFlop : Component
{
    public Terminal D = null!;
    public Terminal Clk = null!;
    public Terminal Set = null!;
    public Terminal Clr = null!;
    public Terminal Q = null!;
    public Terminal QBar = null!;

    private LogicState _lastClk = LogicState.Unknown;

    public override void Simulate()
    {
        if (Clr.State == LogicState.High)
        {
            Q.State = LogicState.Low;
            QBar.State = LogicState.High;
            _lastClk = Clk.State;
            return;
        }

        if (Set.State == LogicState.High)
        {
            Q.State = LogicState.High;
            QBar.State = LogicState.Low;
            _lastClk = Clk.State;
            return;
        }

        if (D.State == LogicState.Unknown || Clk.State == LogicState.Unknown)
        {
            Q.State = LogicState.Unknown;
            QBar.State = LogicState.Unknown;
            _lastClk = Clk.State;
            return;
        }

        if (_lastClk == LogicState.Low && Clk.State == LogicState.High)
        {
            Q.State = D.State;
            QBar.State = D.State == LogicState.High ? LogicState.Low : LogicState.High;
        }

        _lastClk = Clk.State;
    }

    public override void Reset()
    {
        Q.State = LogicState.Unknown;
        QBar.State = LogicState.Unknown;
        _lastClk = LogicState.Unknown;
    }
}
