using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class FullAdder : Component
{
    public Terminal A = null!;
    public Terminal B = null!;
    public Terminal Cin = null!;
    public Terminal Sum = null!;
    public Terminal Cout = null!;

    public override void Simulate()
    {
        if (
            A.State == LogicState.Unknown
            || B.State == LogicState.Unknown
            || Cin.State == LogicState.Unknown
        )
        {
            Sum.State = LogicState.Unknown;
            Cout.State = LogicState.Unknown;
            return;
        }

        int a = A.State == LogicState.High ? 1 : 0;
        int b = B.State == LogicState.High ? 1 : 0;
        int c = Cin.State == LogicState.High ? 1 : 0;

        int total = a + b + c;

        Sum.State = total % 2 == 1 ? LogicState.High : LogicState.Low;
        Cout.State = total >= 2 ? LogicState.High : LogicState.Low;
    }

    public override void Reset()
    {
        Sum.State = LogicState.Unknown;
        Cout.State = LogicState.Unknown;
    }
}
