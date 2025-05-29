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
        // If there's a missing wire, don't bother
        if (Terminals.Any(p => p.Wire == null)) return;

        // Funny LINQ expression
        // Check if number of HIGH inputs is odd
        if (Terminals.SkipLast(1).Where(p => p.Wire.Value == LogicState.High).Count() % 2 != 0)
        {
            Terminals[^1].Wire.Value = LogicState.High;
        }
        else Terminals[^1].Wire.Value = LogicState.Low;

    }
    

   
}