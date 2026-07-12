using System;
using System.Collections.Generic;
using Avalonia.Collections;
using IRis.Models.Core;
using IRis.ViewModels.Main.Canvas;
using IRis.ViewModels.Main.Canvas.CircuitObjects;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components;
using IRis.ViewModels.Main.Canvas.CircuitObjects.Components.Gates;
using IRis.ViewModels.Main.Canvas.Core;

namespace IRis.Services.Commands;

public class DeleteCommand(
    AvaloniaList<CircuitObjectViewModel> originalList,
    AvaloniaList<CircuitObjectViewModel> toRemove
) : CommandBase
{
    private readonly AvaloniaList<CircuitObjectViewModel> _originalList = originalList;
    private readonly AvaloniaList<CircuitObjectViewModel> _collection = [.. toRemove];
    private readonly List<(WireViewModel wire, TerminalViewModel terminal)> _removedOutputs = [];
    private readonly List<(CircuitObjectViewModel item, int index)> _originalIndices = [];
    private readonly List<(
        WireViewModel wire,
        bool isInput,
        TerminalViewModel old
    )> _disconnected = [];

    public override void Execute()
    {
        AvaloniaList<WireViewModel> findAttachedWires(TerminalViewModel terminal)
        {
            AvaloniaList<WireViewModel> wires = [];

            foreach (CircuitObjectViewModel co in _originalList)
            {
                if (co is WireViewModel w && (w.MainInput == terminal || w.MainOutput == terminal))
                    wires.Add(w);
            }

            return wires;
        }

        _disconnected.Clear();

        foreach (CircuitObjectViewModel co in _collection)
        {
            if (co is GateViewModel gate)
            {
                foreach (WireViewModel wire in findAttachedWires(gate.Output))
                {
                    _disconnected.Add((wire, true, wire.MainInput));
                    wire.MainInput = new() { IsOrphan = true };
                }

                AvaloniaList<TerminalViewModel> inputs = [];

                if (gate is MultiInputGateViewModel mig)
                {
                    foreach (TerminalViewModel input in mig.Inputs)
                    {
                        inputs.Add(input);
                    }
                }
                else if (gate is NotGateViewModel notGate)
                {
                    inputs.Add(notGate.Input);
                }

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is FullAdderViewModel fa)
            {
                AvaloniaList<TerminalViewModel> inputs = [fa.A, fa.B, fa.Cin];
                AvaloniaList<TerminalViewModel> outputs = [fa.Sum, fa.Cout];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is DLatchViewModel dl)
            {
                AvaloniaList<TerminalViewModel> inputs = [dl.D, dl.En];
                AvaloniaList<TerminalViewModel> outputs = [dl.Q, dl.QBar];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is DFlipFlopViewModel dff)
            {
                AvaloniaList<TerminalViewModel> inputs = [dff.D, dff.Clk, dff.Set, dff.Clr];
                AvaloniaList<TerminalViewModel> outputs = [dff.Q, dff.QBar];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is JKFlipFlopViewModel jk)
            {
                AvaloniaList<TerminalViewModel> inputs = [jk.J, jk.K, jk.Clk, jk.Set, jk.Clr];
                AvaloniaList<TerminalViewModel> outputs = [jk.Q, jk.QBar];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is TFlipFlopViewModel tff)
            {
                AvaloniaList<TerminalViewModel> inputs = [tff.T, tff.Clk, tff.Set, tff.Clr];
                AvaloniaList<TerminalViewModel> outputs = [tff.Q, tff.QBar];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is RegisterViewModel reg)
            {
                AvaloniaList<TerminalViewModel> inputs = [.. reg.Inputs, reg.Clk, reg.Set, reg.Clr];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in reg.Outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is CounterViewModel counter)
            {
                AvaloniaList<TerminalViewModel> inputs =
                [
                    .. counter.Inputs,
                    counter.Clk,
                    counter.Clr,
                    counter.Load,
                    counter.Enable,
                ];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                AvaloniaList<TerminalViewModel> outputs = [.. counter.Outputs, counter.Carry];

                foreach (TerminalViewModel output in outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is MultiplexerViewModel mux)
            {
                AvaloniaList<TerminalViewModel> inputs = [.. mux.Inputs, .. mux.Selects];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (WireViewModel wire in findAttachedWires(mux.Output))
                {
                    _disconnected.Add((wire, true, wire.MainInput));
                    wire.MainInput = new() { IsOrphan = true };
                }
            }
            else if (co is DemultiplexerViewModel demux)
            {
                AvaloniaList<TerminalViewModel> inputs = [demux.Input, .. demux.Selects];

                foreach (TerminalViewModel input in inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in demux.Outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is DecoderViewModel decoder)
            {
                foreach (TerminalViewModel input in decoder.Selects)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in decoder.Outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is PriorityEncoderViewModel encoder)
            {
                foreach (TerminalViewModel input in encoder.Inputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(input))
                    {
                        _disconnected.Add((wire, false, wire.MainOutput));
                        wire.MainOutput = new() { IsOrphan = true };
                    }
                }

                foreach (TerminalViewModel output in encoder.Outputs)
                {
                    foreach (WireViewModel wire in findAttachedWires(output))
                    {
                        _disconnected.Add((wire, true, wire.MainInput));
                        wire.MainInput = new() { IsOrphan = true };
                    }
                }
            }
            else if (co is ToggleViewModel toggle)
            {
                foreach (WireViewModel wire in findAttachedWires(toggle.Output))
                {
                    _disconnected.Add((wire, true, wire.MainInput));
                    wire.MainInput = new() { IsOrphan = true };
                }
            }
            else if (co is ClockViewModel clock)
            {
                foreach (WireViewModel wire in findAttachedWires(clock.Output))
                {
                    _disconnected.Add((wire, true, wire.MainInput));
                    wire.MainInput = new() { IsOrphan = true };
                }
            }
            else if (co is ProbeViewModel probe)
            {
                foreach (WireViewModel wire in findAttachedWires(probe.Input))
                {
                    _disconnected.Add((wire, false, wire.MainOutput));
                    wire.MainOutput = new() { IsOrphan = true };
                }
            }
        }

        foreach (CircuitObjectViewModel co in _collection)
        {
            if (co is WireViewModel wire)
            {
                wire.MainInput.GetModel().State = LogicState.Unknown;
                wire.MainOutput.GetModel().State = LogicState.Unknown;
            }
        }

        _removedOutputs.Clear();
        _originalIndices.Clear();

        foreach (var item in _collection)
            _originalIndices.Add((item, _originalList.IndexOf(item)));

        _originalList.RemoveAll(_collection);

        foreach (var item in _collection)
        {
            if (item is not WireViewModel removedWire)
                continue;

            foreach (var obj in _originalList)
            {
                if (obj is not WireViewModel wire)
                    continue;

                foreach (var output in wire.Outputs)
                {
                    if (output == removedWire.MainOutput || output == removedWire.MainInput)
                        _removedOutputs.Add((wire, output));
                }
            }
        }

        foreach (var (wire, terminal) in _removedOutputs)
        {
            wire.Outputs.Remove(terminal);
        }
    }

    public override void Undo()
    {
        foreach (var (item, index) in _originalIndices)
            _originalList.Insert(Math.Min(index, _originalList.Count), item);

        foreach (var (wire, isInput, old) in _disconnected)
        {
            if (isInput)
                wire.MainInput = old;
            else
                wire.MainOutput = old;
        }

        foreach (var (wire, terminal) in _removedOutputs)
        {
            wire.Outputs.Add(terminal);
        }
    }
}
