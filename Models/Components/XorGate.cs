using System.Linq;
using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;


namespace  IRis.Models.Components;


public class XorGate : Gate
{
    public XorGate(int numInputs) : base(numInputs)
    {

    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);

        this.DrawOr(ctx, true);

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

        // XOR logic: output is HIGH when an odd number of inputs are HIGH
        int highInputCount = inputValues.Count(value => value == true);
        LogicState outputValue = (highInputCount % 2 != 0) ? LogicState.High : LogicState.Low;

        // Set output on ALL connected wires
        foreach (var wire in outputTerminal.Wires)
        {
            wire.Value = outputValue;
        }
    }
    

   
}