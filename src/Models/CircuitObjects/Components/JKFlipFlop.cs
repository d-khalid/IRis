using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class JKFlipFlop : Component
{
    public Terminal J = null!;
    public Terminal K = null!;
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

        if (
            J.State == LogicState.Unknown
            || K.State == LogicState.Unknown
            || Clk.State == LogicState.Unknown
        )
        {
            Q.State = LogicState.Unknown;
            QBar.State = LogicState.Unknown;
            _lastClk = Clk.State;
            return;
        }

        if (_lastClk == LogicState.Low && Clk.State == LogicState.High)
        {
            if (J.State == LogicState.High && K.State == LogicState.Low)
            {
                Q.State = LogicState.High;
                QBar.State = LogicState.Low;
            }
            else if (J.State == LogicState.Low && K.State == LogicState.High)
            {
                Q.State = LogicState.Low;
                QBar.State = LogicState.High;
            }
            else if (J.State == LogicState.High && K.State == LogicState.High)
            {
                if (Q.State == LogicState.High)
                {
                    Q.State = LogicState.Low;
                    QBar.State = LogicState.High;
                }
                else
                {
                    Q.State = LogicState.High;
                    QBar.State = LogicState.Low;
                }
            }
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
