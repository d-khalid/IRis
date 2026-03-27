using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


namespace IRis.Models.Components;


public class OrGate : Gate
{
    public OrGate(int numInputs) : base(numInputs)
    {

    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);

        DrawOr(ctx);

        base.Draw(ctx);


    }

    public override void ComputeOutput()
    {
        // For inputs: check if ANY input terminal has at least one wire
        // For output: check if output terminal has at least one wire
        if (Terminals == null) return;
        var inputTerminals = Terminals.SkipLast(1);
        var outputTerminal = Terminals[^1];

        if (!inputTerminals.All(t => t.Wires.Any()) || !outputTerminal.Wires.Any()) return;

        // For each input terminal, OR together all connected wire values, then OR all inputs
        bool anyInputHigh = inputTerminals.Any(terminal =>
            terminal.Wires.Any(w => w.Value == LogicState.High));

        // Set output on ALL connected wires
        LogicState outputValue = anyInputHigh ? LogicState.High : LogicState.Low;
        foreach (var wire in outputTerminal.Wires)
        {
            wire.Value = outputValue;
        }
    }
}