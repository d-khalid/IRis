using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


namespace  IRis.Models.Components;


public class AndGate : Gate
{
    public AndGate(int numInputs) : base(numInputs)
    {

    }

    public override void Draw(DrawingContext ctx)
    {
        // 3. Draw terminals (lines + circles)
        DrawTerminals(ctx);

        this.DrawAnd(ctx);

        base.Draw(ctx);
    }

    public override void ComputeOutput()
    {
        // For inputs: check if ANY input terminal has at least one wire
        // For output: check if output terminal has at least one wire
        var inputTerminals = Terminals.SkipLast(1);
        var outputTerminal = Terminals[^1];

        if (!inputTerminals.All(t => t.Wires.Any()) || !outputTerminal.Wires.Any()) return;

        // For each input terminal, OR together all connected wire values
        var inputValues = inputTerminals.Select(terminal => 
                        terminal.Wires.Any(w => w.Value == LogicState.High)).ToList();

        if (inputValues.All(value => value == true)) // All inputs must be high
        {
            foreach (var wire in outputTerminal.Wires)
            {
                wire.Value = LogicState.High;
            }
        }
        else
        {
            foreach (var wire in outputTerminal.Wires)
            {
                wire.Value = LogicState.Low;
            }
        }
    }
}