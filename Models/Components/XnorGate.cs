using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


namespace IRis.Models.Components;


public class XnorGate : Gate
{
    public XnorGate(int numInputs) : base(numInputs, notMode: true)
    {

    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);

        DrawOr(ctx, true);

        // 3. Draw the bubble at the end
        ctx.DrawEllipse(
            Brushes.White, // Fill (none)
            ComponentDefaults.GatePen, // Use same pen as gate
            new Point(Width + ComponentDefaults.BubbleRadius, Height / 2),
            ComponentDefaults.BubbleRadius,
            ComponentDefaults.BubbleRadius);

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

        // For each input terminal, OR together all connected wire values
        var inputValues = inputTerminals.Select(terminal =>
            terminal.Wires.Any(w => w.Value == LogicState.High)).ToList();

        // XNOR logic: output is HIGH when an even number of inputs are HIGH (inverted XOR)
        int highInputCount = inputValues.Count(value => value);
        LogicState outputValue = (highInputCount % 2 != 0) ? LogicState.Low : LogicState.High;

        // Set output on ALL connected wires
        foreach (var wire in outputTerminal.Wires)
        {
            wire.Value = outputValue;
        }
    }

}