using Avalonia;
using Avalonia.Media;
using IRis.Models.Core;
using System.Linq;


namespace  IRis.Models.Components;


public class OrGate : Gate
{
    public OrGate(int numInputs) : base(numInputs)
    {

    }

    public override void Draw(DrawingContext ctx)
    {

        // 3. Draw terminals (input left, output right)
        DrawTerminals(ctx);

        this.DrawOr(ctx);

        base.Draw(ctx);


    }
    
    public override void ComputeOutput()
    {
        // If there's a missing wire, don't bother
        if (Terminals.Any(p => p.Wire == null)) return;

        // Funny LINQ expression
        if (Terminals.SkipLast(1).Any(p => p.Wire.Value == LogicState.High))
        {
            Terminals[^1].Wire.Value = LogicState.High;
        }
        else Terminals[^1].Wire.Value = LogicState.Low;

    }
    
    

    // public override void UpdateOutputValue()
    // {
    //     var values = Inputs.Select(input => input.Wire.Value).ToArray();
    //
    //     // Logic applies for any no. of Inputs
    //     if (values.Any(v => v == null)) Output.Wire.Value = null;
    //     else Output.Wire.Value = values.Any(v => v == true);
    // }
}