using System.Collections.Generic;
using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class Register : Component
{
    public List<Terminal> Inputs { get; } = [];
    public List<Terminal> Outputs { get; } = [];
    public List<LogicState> States { get; set; } = [];
    public Terminal Clk = null!;
    public Terminal Set = null!;
    public Terminal Clr = null!;

    private LogicState _lastClk = LogicState.Unknown;

    public override void Simulate()
    {
        while (States.Count < Outputs.Count)
            States.Add(LogicState.Unknown);
        while (States.Count > Outputs.Count)
            States.RemoveAt(States.Count - 1);

        if (Clr.State == LogicState.High)
        {
            for (int i = 0; i < States.Count; i++)
                States[i] = LogicState.Low;

            PushStates();
            _lastClk = Clk.State;
            return;
        }

        if (Set.State == LogicState.High)
        {
            for (int i = 0; i < States.Count; i++)
                States[i] = LogicState.High;

            PushStates();
            _lastClk = Clk.State;
            return;
        }

        if (Clk.State == LogicState.Unknown)
        {
            for (int i = 0; i < States.Count; i++)
                States[i] = LogicState.Unknown;

            PushStates();
            _lastClk = Clk.State;
            return;
        }

        if (_lastClk == LogicState.Low && Clk.State == LogicState.High)
        {
            for (int i = 0; i < Inputs.Count && i < States.Count; i++)
            {
                if (Inputs[i].State == LogicState.Unknown)
                    States[i] = LogicState.Unknown;
                else
                    States[i] = Inputs[i].State;
            }
        }

        PushStates();
        _lastClk = Clk.State;
    }

    public override void Reset()
    {
        for (int i = 0; i < States.Count; i++)
            States[i] = LogicState.Unknown;

        PushStates();
        _lastClk = LogicState.Unknown;
    }

    private void PushStates()
    {
        for (int i = 0; i < Outputs.Count && i < States.Count; i++)
            Outputs[i].State = States[i];
    }
}
