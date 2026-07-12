using System.Collections.Generic;
using IRis.Models.Core;

namespace IRis.Models.CircuitObjects.Components;

public class Counter : Component
{
    public List<Terminal> Inputs { get; } = [];
    public List<Terminal> Outputs { get; } = [];
    public List<LogicState> States { get; set; } = [];
    public Terminal Clk = null!;
    public Terminal Clr = null!;
    public Terminal Load = null!;
    public Terminal Enable = null!;
    public Terminal Carry = null!;

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

            Carry.State = LogicState.Low;
            PushStates();
            _lastClk = Clk.State;
            return;
        }

        if (Clk.State == LogicState.Unknown)
        {
            for (int i = 0; i < States.Count; i++)
                States[i] = LogicState.Unknown;

            Carry.State = LogicState.Unknown;
            PushStates();
            _lastClk = Clk.State;
            return;
        }

        if (_lastClk == LogicState.Low && Clk.State == LogicState.High)
        {
            if (Load.State == LogicState.High)
            {
                for (int i = 0; i < Inputs.Count && i < States.Count; i++)
                {
                    if (Inputs[i].State == LogicState.Unknown)
                        States[i] = LogicState.Unknown;
                    else
                        States[i] = Inputs[i].State;
                }

                Carry.State = LogicState.Low;
            }
            else if (Enable.State == LogicState.High)
            {
                if (HasUnknownState())
                {
                    for (int i = 0; i < States.Count; i++)
                        States[i] = LogicState.Unknown;

                    Carry.State = LogicState.Unknown;
                }
                else
                {
                    int value = ToInt();
                    int max = (1 << States.Count) - 1;

                    if (value == max)
                    {
                        for (int i = 0; i < States.Count; i++)
                            States[i] = LogicState.Low;

                        Carry.State = LogicState.High;
                    }
                    else
                    {
                        FromInt(value + 1);
                        Carry.State = LogicState.Low;
                    }
                }
            }
            else
            {
                Carry.State = LogicState.Low;
            }
        }

        PushStates();
        _lastClk = Clk.State;
    }

    public override void Reset()
    {
        for (int i = 0; i < States.Count; i++)
            States[i] = LogicState.Unknown;

        Carry.State = LogicState.Unknown;
        PushStates();
        _lastClk = LogicState.Unknown;
    }

    private bool HasUnknownState()
    {
        for (int i = 0; i < States.Count; i++)
        {
            if (States[i] == LogicState.Unknown)
                return true;
        }

        return false;
    }

    private int ToInt()
    {
        int value = 0;

        for (int i = 0; i < States.Count; i++)
        {
            if (States[i] == LogicState.High)
                value |= 1 << i;
        }

        return value;
    }

    private void FromInt(int value)
    {
        for (int i = 0; i < States.Count; i++)
            States[i] = (value & (1 << i)) != 0 ? LogicState.High : LogicState.Low;
    }

    private void PushStates()
    {
        for (int i = 0; i < Outputs.Count && i < States.Count; i++)
            Outputs[i].State = States[i];
    }
}
