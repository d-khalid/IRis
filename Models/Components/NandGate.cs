using System.Linq;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class NandGate : Gate
{
    public NandGate(int numInputs) : base(numInputs, notMode: true)
    {

    }

    public override void Draw(DrawingContext ctx)
    {
        // 3. Draw terminals (lines + circles)
        DrawTerminals(ctx);

        this.DrawAnd(ctx);

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

        // NAND logic: NOT(AND) - output is LOW only when ALL inputs are HIGH
        LogicState outputValue = inputValues.All(value => value == true) ? LogicState.Low : LogicState.High;

        // Set output on ALL connected wires
        foreach (var wire in outputTerminal.Wires)
        {
            wire.Value = outputValue;
        }
    }
    
    
}